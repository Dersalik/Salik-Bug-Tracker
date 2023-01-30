using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/v{version:apiVersion}/Projects/{ProjectId}/Modules/{ModuleId}/Developers")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        private readonly ILogger<DevelopersController> _logger;

        public DevelopersController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DevelopersController> logger)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        /// <summary>
        /// assigns developer to a bug 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ModuleId"></param>
        /// <param name="DeveloperId"></param>
        [HttpPost("{DeveloperId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> AssignDeveloper(int ProjectId,int ModuleId, string DeveloperId)
        {
            try {
                _logger.LogInformation("Received request to assign developer {DeveloperId} to module {ModuleId} in project {ProjectId}", DeveloperId, ModuleId, ProjectId);
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning($"project with id {ProjectId} wasnt found ");
                return NotFound();
            }
            bool IsModuleAvailable=await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

            if (!IsModuleAvailable)
            {
                _logger.LogWarning($"module wasnt not found with id {ModuleId} of project with id {ProjectId}");
                return NotFound();
            }

            var devToAssignToModule = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == DeveloperId);

            if(devToAssignToModule == null)
            {
                _logger.LogWarning($"Failed to find a developer with id {ProjectId}");
                return NotFound();
            }

            var checkIfDevAlreadyAssigned = await _unitOfWork.moduleUserRepository.checkIfModuleAlreadyAssignedToDev(ModuleId, DeveloperId);
            if (checkIfDevAlreadyAssigned)
            {
                    _logger.LogWarning($"Developer with id {DeveloperId} is already assigned to a module with id {ModuleId}");
                return BadRequest("Dev already assigned");
            }
            var ModuleToAssignDevTo = await _unitOfWork.moduleRepository.GetFirstOrDefault(d => d.Id == ModuleId);
            ModuleUser newModuleUser=new ModuleUser { user= devToAssignToModule, module= ModuleToAssignDevTo };
            
            await _unitOfWork.moduleUserRepository.Add(newModuleUser);    

            await _unitOfWork.Save();
                _logger.LogInformation($"Developer with id {DeveloperId} was  assigned to a module with id {ModuleId}");
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while trying to assign a Developer with id {DeveloperId} to a module with id {ModuleId}:{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }


        }
        /// <summary>
        /// Unassigns developer from a bug 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ModuleId"></param>
        /// <param name="DeveloperId"></param>
\        [HttpDelete("{DeveloperId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UnassignDeveloper(int ProjectId, int ModuleId, string DeveloperId)
        {
            try {
                _logger.LogInformation("Received request to unassign developer {DeveloperId} from module {ModuleId} in project {ProjectId}", DeveloperId, ModuleId, ProjectId);

                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning($"project with id {ProjectId} wasnt found ");
                    return NotFound();
            }
            bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

            if (!IsModuleAvailable)
            {
                    _logger.LogWarning($"module wasnt not found with id {ModuleId} of project with id {ProjectId}");
                    return NotFound();
            }

            var devToAssignToModule = await _unitOfWork.userRepository.GetFirstOrDefaultWithAllAttributes(d => d.Id == DeveloperId);

            if (devToAssignToModule == null)
            {
                    _logger.LogWarning($"Failed to find a developer with id {ProjectId}");
                    return NotFound();
            }
            var checkIfDevAlreadyAssigned = await _unitOfWork.moduleUserRepository.checkIfModuleAlreadyAssignedToDev(ModuleId, DeveloperId);
            if (!checkIfDevAlreadyAssigned)
            {
                    _logger.LogWarning($"Developer with id {DeveloperId} is not assigned to a module with id {ModuleId},therefore cant be unassigned");
                    return BadRequest("Dev is not assigned, therefore cant be unassigned");
            }

           var ModuleUserToDelete= await _unitOfWork.moduleUserRepository.GetFirstOrDefault(d=>d.ModuleId==ModuleId && d.ApplicationUserId==DeveloperId );
           
            _unitOfWork.moduleUserRepository.Remove(ModuleUserToDelete);
            await _unitOfWork.Save();
                _logger.LogInformation($"A developer with id {DeveloperId} was successfuly unassigned from a module with id {ModuleId}");
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred while trying to unassign a developer with id {DeveloperId} from a module with id {ModuleId}:{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }


        }
        /// <summary>
        /// gets developers that are assigned to a module 
        /// </summary>
        /// <param name="ModuleId"></param>
\        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDTO>>> getDevs(int ModuleId)
        {
            try {
                _logger.LogInformation("Received request to retrieve developers assigned to module {ModuleId}", ModuleId);

                var result = await _unitOfWork.userRepository.getDevsOfOneModule(ModuleId);
                _logger.LogInformation($"all the developer of module with id {ModuleId} was returned successfully");
                return Ok(Mapper.Map<List<UserDTO>>(result)); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while trying to return all the developer of a module with id {ModuleId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

    }
}
