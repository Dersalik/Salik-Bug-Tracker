using AutoMapper;
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

namespace Salik_Bug_Tracker_API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
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

        [HttpGet("{developerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                _logger.LogError($"An error occurred while retrieving developer with id {developerId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                _logger.LogError($"An error occurred while retrieving all developers: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

       
            [HttpGet("{developerId}/modules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                _logger.LogError($"An error occurred while retrieving modules for developer with id {developerId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{developerId}/bugs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                _logger.LogError($"An error occurred while retrieving bugs for developer with id {developerId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{ModuleId}/bugs/{developerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BugDTO>>> GetBugsByModuleIdAndDeveloperId(int ModuleId, string developerId)
        {
            try
            {
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
                _logger.LogError($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }


}