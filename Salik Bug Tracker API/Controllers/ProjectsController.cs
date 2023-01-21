using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        const int maxProjectPageSize = 14;

        public ProjectsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.Mapper = mapper;
            this._unitOfWork = unitOfWork;
        }


        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> getProjects([FromQuery]
            string? name, [FromQuery] string? searchQuery, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageSize > maxProjectPageSize)
                {
                    pageSize = maxProjectPageSize;
                }

                var (projects, paginationMetadata) = await _unitOfWork.projectRepository.GetProjectsAsync(name, searchQuery, pageNumber, pageSize);
                Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
                var test = Mapper.Map<IEnumerable<ProjectDTO>>(projects);
                return Ok(Mapper.Map<IEnumerable<ProjectDTO>>(projects));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }


        [HttpGet("{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDTO>> getProject(int projectId)
        {
            try
            {
                var projectExists = await _unitOfWork.projectRepository.CheckProjectExists(projectId);

                if (!projectExists)
                {
                    return NotFound(projectId);
                }
                var result = await _unitOfWork.projectRepository.GetFirstOrDefault(d => d.Id == projectId);
                return Ok(Mapper.Map<ProjectDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPut("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] ProjectForUpdateDTO projectDto)
        {
            try
            {
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var resultOfMapping = Mapper.Map<Project>(projectDto);
                _unitOfWork.projectRepository.UpdateEntity(resultOfMapping);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }

        [HttpPatch("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] JsonPatchDocument<ProjectForUpdateDTO> patchDocument)
        {
            try
            {
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    return NotFound();
                }

                var projectFromRepo = await _unitOfWork.projectRepository.GetFirstOrDefault(p => p.Id == ProjectId);

                if (projectFromRepo == null)
                {
                    return NotFound();
                }

                var projectToPatch = Mapper.Map<ProjectForUpdateDTO>(projectFromRepo);
                patchDocument.ApplyTo(projectToPatch, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Mapper.Map(projectToPatch, projectFromRepo);
                _unitOfWork.projectRepository.UpdateEntity(projectFromRepo);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database" );
            }

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectForCreationDTO projectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var projectToCreate = Mapper.Map<Project>(projectDto);
                await _unitOfWork.projectRepository.Add(projectToCreate);
                await _unitOfWork.Save();

                var projectToReturn = Mapper.Map<ProjectDTO>(projectToCreate);
                return CreatedAtAction(nameof(getProject), new { projectId = projectToReturn.Id }, projectToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data in the database");
            }
        }

        [HttpDelete("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> deleteProject(int ProjectId)
        {
            try
            {
                if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
                {
                    return NotFound();
                }

                var projectFromRepo = await _unitOfWork.projectRepository.GetFirstOrDefault(p => p.Id == ProjectId);

                 _unitOfWork.projectRepository.Remove(projectFromRepo);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }
    }
}
