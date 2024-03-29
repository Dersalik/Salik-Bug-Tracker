﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using System;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private readonly IMemoryCache _memoryCache;

        private IUnitOfWork _unitOfWork { get; }
        const int maxProjectPageSize = 14;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ProjectsController> logger, IMemoryCache memoryCache)
        {
            this.Mapper = mapper;
            this._unitOfWork = unitOfWork;
            _logger = logger;
            _memoryCache = memoryCache;

        }

        /// <summary>
        /// gets list of the projects 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="searchQuery"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects([FromQuery]
    string? name, [FromQuery] string? searchQuery, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Retrieving Projects with Name: {name}, Search Query: {searchQuery}, Page Number: {pageNumber}, and Page Size: {pageSize}.");
                if (pageSize > maxProjectPageSize)
                {
                    pageSize = maxProjectPageSize;
                }

                // Check the cache for the results first
                var cacheKey = $"GetProjects_{name}_{searchQuery}_{pageNumber}_{pageSize}";
                var cachedResult = _memoryCache.Get<IEnumerable<ProjectDTO>>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Retrieved data from cache");
                    return Ok(cachedResult);
                }

                var (projects, paginationMetadata) = await _unitOfWork.projectRepository.GetProjectsAsync(name, searchQuery, pageNumber, pageSize);
                Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
                var res = Mapper.Map<IEnumerable<ProjectDTO>>(projects);

                // Store the result in the cache for later
                _memoryCache.Set(cacheKey, res, TimeSpan.FromMinutes(30));

                _logger.LogInformation("Get projects called successfully");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data from the database: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        /// <summary>
        /// Gets a particular project 
        /// </summary>
        /// <param name="projectId"></param>
        [HttpGet("{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProjectDTO>> getProject(int projectId)
        {
            try
            {
                _logger.LogInformation($"Retrieving project with ID: {projectId}");
                var projectExists = await _unitOfWork.projectRepository.CheckProjectExists(projectId);

                if (!projectExists)
                {
                    _logger.LogWarning($"Project with ID {projectId} not found");
                    return NotFound(projectId);
                }
                var result = await _unitOfWork.projectRepository.GetFirstOrDefault(d => d.Id == projectId);
                _logger.LogInformation($"Get project with ID {projectId} called successfully");
                return Ok(Mapper.Map<ProjectDTO>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving project with id {projectId}data from the database: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        /// <summary>
        /// Updates a project 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="projectDto"></param>
        [HttpPut("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] ProjectForUpdateDTO projectDto)
        {
            try
            {
                _logger.LogInformation($"Updating project with id: {ProjectId} - {projectDto} ");
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state");
                    return BadRequest(ModelState);
                }
                var resultOfMapping = Mapper.Map<Project>(projectDto);
                _unitOfWork.projectRepository.UpdateEntity(resultOfMapping);
                await _unitOfWork.Save();
                _logger.LogInformation($"Update project with ID {ProjectId} called successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating project with ID {ProjectId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }
        /// <summary>
        /// Updates a specific attribute of a project 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="patchDocument"></param>
        [HttpPatch("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] JsonPatchDocument<ProjectForUpdateDTO> patchDocument)
        {
            try
            {
                _logger.LogInformation($"Updating project with id: {ProjectId} with patch document: {patchDocument}.");    
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
                }

                var projectFromRepo = await _unitOfWork.projectRepository.GetFirstOrDefault(p => p.Id == ProjectId);

                if (projectFromRepo == null)
                {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
                }

                var projectToPatch = Mapper.Map<ProjectForUpdateDTO>(projectFromRepo);
                patchDocument.ApplyTo(projectToPatch, ModelState);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state");
                    return BadRequest(ModelState);
                }

                Mapper.Map(projectToPatch, projectFromRepo);
                _unitOfWork.projectRepository.UpdateEntity(projectFromRepo);
                await _unitOfWork.Save();
                _logger.LogInformation($"Update project with ID {ProjectId} called successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating project with ID {ProjectId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database" );
            }

        }
        /// <summary>
        /// Creates a new project 
        /// </summary>
        /// <param name="projectDto"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectForCreationDTO projectDto)
        {
            try
            {
                _logger.LogInformation("Creating new project...");
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state");
                    return BadRequest(ModelState);
                }

                var projectToCreate = Mapper.Map<Project>(projectDto);
                await _unitOfWork.projectRepository.Add(projectToCreate);
                await _unitOfWork.Save();
                _logger.LogInformation("Create project called successfully");
                var projectToReturn = Mapper.Map<ProjectDTO>(projectToCreate);
                return CreatedAtAction(nameof(getProject), new { projectId = projectToReturn.Id }, projectToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating project: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data in the database");
            }
        }
        /// <summary>
        /// Deletes a project 
        /// </summary>
        /// <param name="ProjectId"></param>
        [HttpDelete("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> deleteProject(int ProjectId)
        {
            try
            {
                _logger.LogInformation($"deleting Project with {ProjectId}....");
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
                }

                var projectFromRepo = await _unitOfWork.projectRepository.GetFirstOrDefault(p => p.Id == ProjectId);

                 _unitOfWork.projectRepository.Remove(projectFromRepo);
                await _unitOfWork.Save();
                _logger.LogInformation($"Delete project with ID {ProjectId} called successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting project with ID {ProjectId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }
    }
}
