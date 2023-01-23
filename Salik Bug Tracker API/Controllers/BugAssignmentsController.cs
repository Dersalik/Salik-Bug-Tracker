using AutoMapper;
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
    public class BugAssignmentsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }

        public BugAssignmentsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BugDeveloper>> AssignDeveloperToBug(int BugId, string developerId)
        {
            try
            {
                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                var developer = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == developerId);
                if (bug == null || developer == null)
                {
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
                return NoContent();
            }
            catch (Exception ex)
            {
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
                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                if (bug == null)
                {
                    return NotFound("Bug not found");
                }
                var developers = await _unitOfWork.bugDeveloperRepository.GetDevelopersByBugId(BugId);


                var developerDTOs = Mapper.Map<IEnumerable<UserDTO>>(developers);
                return Ok(developerDTOs);
            }
            catch (Exception ex)
            {
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
                var bugAssignment = await _unitOfWork.bugDeveloperRepository.GetFirstOrDefault(ba => ba.BugId == BugId && ba.ApplicationUserId == developerId);
                if (bugAssignment == null)
                {
                    return NotFound("Assignment not found");
                }
                _unitOfWork.bugDeveloperRepository.Remove(bugAssignment);
                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error unassigning developer from bug");
            }
        }


      
    }
}
