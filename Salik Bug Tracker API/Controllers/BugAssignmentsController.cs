using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/v{version:apiVersion}/Projects/{ProjectId}/Modules/{ModuleId}/Bugs/{BugId}/BugAssignments")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class BugAssignmentsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        private readonly ILogger<BugAssignmentsController> _logger;

        public BugAssignmentsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<BugAssignmentsController> logger)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BugDeveloper>> AssignDeveloperToBug(int BugId, string developerId)
        {
            try
            {
                _logger.LogInformation("Received request to assign developer {developerId} to bug {BugId}", developerId, BugId);

                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                var developer = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == developerId);
                if (bug == null || developer == null)
                {
                    _logger.LogWarning($"failed to retreive a bug with id {BugId} or developer with id {developerId}");
                    return NotFound("Bug or developer not found");
                }
                var bugAssignment = new BugDeveloper
                {
                    BugId = bug.Id,
                    bug = bug,
                    ApplicationUserId = developer.Id,
                    user = developer
                };
                await _unitOfWork.bugDeveloperRepository.Add(bugAssignment);
                await _unitOfWork.Save();
                _logger.LogInformation($"Successfuly developer with id {developerId} was assigned to a bug with id {BugId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while trying to assing a developer with id {developerId} to a bug with id {BugId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error assigning developer to bug");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetBugAssignments(int BugId)
        {
            try
            {
                _logger.LogInformation("Received request to retrieve assignments for bug {BugId}", BugId);

                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                if (bug == null)
                {
                    _logger.LogWarning($"failed to retreive a bug with id {BugId} ");
                    return NotFound("Bug not found");
                }
                var developers = await _unitOfWork.bugDeveloperRepository.GetDevelopersByBugId(BugId);


                var developerDTOs = Mapper.Map<IEnumerable<UserDTO>>(developers);
                _logger.LogInformation($"{developerDTOs.Count()} developers that were assigned to a bug with id {BugId} were returned ");
                return Ok(developerDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred while trying to retrieve developers assigned to a bug with id {BugId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving developers assigned to the bug");
            }
        }

        [HttpDelete("{developerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignDeveloperFromBug(int BugId, string developerId)
        {
            try
            {
                _logger.LogInformation("Received request to unassign developer {developerId} from bug {BugId}", developerId, BugId);

                var bugAssignment = await _unitOfWork.bugDeveloperRepository.GetFirstOrDefault(ba => ba.BugId == BugId && ba.ApplicationUserId == developerId);
                if (bugAssignment == null)
                {
                    _logger.LogWarning($"no developers with id {developerId} was found assigned to a bug with id {BugId}");
                    return NotFound("Assignment not found");
                }
                _unitOfWork.bugDeveloperRepository.Remove(bugAssignment);
                await _unitOfWork.Save();
                _logger.LogInformation($"developer with id {developerId} was successfully unassigned from a bug with id {BugId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred while trying to unassign developer with id {developerId} from bug with id {BugId} : {ex} ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error unassigning developer from bug");
            }
        }


      
    }
}
