using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using AutoMapper;

namespace Salik_Bug_Tracker_API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{developerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetDeveloper(string developerId)
        {
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    return NotFound(developerId);
                }

                var result = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == developerId);
                return Ok(_mapper.Map<UserDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDTO>>> GetDevelopers()
        {
            try
            {
                var result = await _unitOfWork.userRepository.GetAll();
                return Ok(_mapper.Map<List<UserDTO>>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{developerId}/modules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetModulesByDeveloperId(string developerId)
        {
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    return NotFound(developerId);
                }

                var modules = await _unitOfWork.moduleRepository.GetModulesByDeveloperId(developerId);
                return Ok(_mapper.Map<IEnumerable<ModuleDTO>>(modules));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{developerId}/bugs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BugDTO>>> GetBugsByDeveloperId(string developerId)
        {
            try
            {
                var devExists = await _unitOfWork.userRepository.CheckDevExists(developerId);

                if (!devExists)
                {
                    return NotFound(developerId);
                }

                var bugs = await _unitOfWork.bugRepository.GetBugsByDeveloperId(developerId);
                return Ok(_mapper.Map<IEnumerable<BugDTO>>(bugs));
            }
            catch (Exception ex)
            {
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
                    return NotFound("Module or developer not found");
                }

                var bugs = await _unitOfWork.bugRepository.GetBugsByModuleIdAndDeveloperId(ModuleId, developerId);
                return Ok(_mapper.Map<IEnumerable<BugDTO>>(bugs));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }

}