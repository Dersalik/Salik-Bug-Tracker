﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Salik_Bug_Tracker_API.Models.Helpers;

namespace Salik_Bug_Tracker_API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Developer)]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UsersController> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        /// <summary>
        /// Gets a particular developer
        /// </summary>
        /// <param name="developerId"></param>
        [HttpGet("{developerId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserDTO>> GetDeveloper(string developerId)
        {
            _logger.LogInformation($"Retrieving developer with id {developerId}");
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    _logger.LogWarning($"Developer with id {developerId} was not found");
                    return NotFound(developerId);
                }

                var result = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == developerId);
                _logger.LogInformation($"Retrieved developer with id {developerId}");
                return Ok(_mapper.Map<UserDTO>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving developer with id {developerId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// Gets all of the developers 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<UserDTO>>> GetDevelopers()
        {
            _logger.LogInformation("Retrieving all developers");
            try
            {
                var result = await _unitOfWork.userRepository.GetAll();
                _logger.LogInformation("Retrieved all developers");
                return Ok(_mapper.Map<List<UserDTO>>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving all developers: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

       /// <summary>
       /// gets all of the modules that are assigned to a particular Developer
       /// </summary>
       /// <param name="developerId"></param>
        [HttpGet("{developerId}/modules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetModulesByDeveloperId(string developerId)
        {
            _logger.LogInformation($"Retrieving modules for developer with id {developerId}");
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    _logger.LogWarning($"Developer with id {developerId} was not found");
                    return NotFound(developerId);
                }

                var modules = await _unitOfWork.moduleRepository.GetModulesByDeveloperId(developerId);
                _logger.LogInformation($"Retrieved modules for developer with id {developerId}");
                return Ok(_mapper.Map<IEnumerable<ModuleDTO>>(modules));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving modules for developer with id {developerId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        ///  gets all of the bugs that are assigned to a particular Developer
        /// </summary>
        /// <param name="developerId"></param>
        [HttpGet("{developerId}/bugs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<BugDTO>>> GetBugsByDeveloperId(string developerId)
        {
            _logger.LogInformation($"Retrieving bugs for developer with id {developerId}");
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    _logger.LogWarning($"Developer with id {developerId} was not found");
                    return NotFound(developerId);
                }

                var bugs = await _unitOfWork.bugRepository.GetBugsByDeveloperId(developerId);
                _logger.LogInformation($"Retrieved bugs for developer with id {developerId}");
                return Ok(_mapper.Map<IEnumerable<BugDTO>>(bugs));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving bugs for developer with id {developerId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// gets bugs that are in a particular module and are assigned to a particular developer 
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="developerId"></param>
        [HttpGet("{ModuleId}/bugs/{developerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<BugDTO>>> GetBugsByModuleIdAndDeveloperId(int ModuleId, string developerId)
        {
            try
            {
                _logger.LogInformation($"getting bugs that are assigned developer with id {developerId} found in module with id {ModuleId}");

                var moduleExists = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

             
                if (!moduleExists || !devExists)
                {
                    _logger.LogError("Module or developer not found");
                    return NotFound("Module or developer not found");
                }

                var bugs = await _unitOfWork.bugRepository.GetBugsByModuleIdAndDeveloperId(ModuleId, developerId);
                _logger.LogInformation($"Retrieved {bugs.Count()} bugs for moduleId: {ModuleId} and developerId: {developerId}");
                return Ok(_mapper.Map<IEnumerable<BugDTO>>(bugs));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }


}