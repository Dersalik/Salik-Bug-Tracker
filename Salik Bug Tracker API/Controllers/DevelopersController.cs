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

        public DevelopersController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        
        [HttpPost("{DeveloperId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AssignDeveloper(int ProjectId,int ModuleId, string DeveloperId)
        {
            try {   bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }
            bool IsModuleAvailable=await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

            if (!IsModuleAvailable)
            {
                return NotFound();
            }

            var devToAssignToModule = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == DeveloperId);

            if(devToAssignToModule == null)
            {
                return NotFound();
            }

            var checkIfDevAlreadyAssigned = await _unitOfWork.moduleUserRepository.checkIfModuleAlreadyAssignedToDev(ModuleId, DeveloperId);
            if (checkIfDevAlreadyAssigned)
            {
                return BadRequest("Dev already assigned");
            }
            var ModuleToAssignDevTo = await _unitOfWork.moduleRepository.GetFirstOrDefault(d => d.Id == ModuleId);
            ModuleUser newModuleUser=new ModuleUser { user= devToAssignToModule, module= ModuleToAssignDevTo };
            
            await _unitOfWork.moduleUserRepository.Add(newModuleUser);    

            await _unitOfWork.Save();

            return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }


        }

        [HttpDelete("{DeveloperId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnassignDeveloper(int ProjectId, int ModuleId, string DeveloperId)
        {
            try { bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }
            bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

            if (!IsModuleAvailable)
            {
                return NotFound();
            }

            var devToAssignToModule = await _unitOfWork.userRepository.GetFirstOrDefaultWithAllAttributes(d => d.Id == DeveloperId);

            if (devToAssignToModule == null)
            {
                return NotFound();
            }
            var checkIfDevAlreadyAssigned = await _unitOfWork.moduleUserRepository.checkIfModuleAlreadyAssignedToDev(ModuleId, DeveloperId);
            if (!checkIfDevAlreadyAssigned)
            {
                return BadRequest("Dev is not assigned, therefore cant be unassigned");
            }

           var ModuleUserToDelete= await _unitOfWork.moduleUserRepository.GetFirstOrDefault(d=>d.ModuleId==ModuleId && d.ApplicationUserId==DeveloperId );
           
            _unitOfWork.moduleUserRepository.Remove(ModuleUserToDelete);
            await _unitOfWork.Save();

            return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }


        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDTO>>> getDevs(int ModuleId)
        {
            try {      
                var result = await _unitOfWork.userRepository.getDevsOfOneModule(ModuleId);
                return Ok(Mapper.Map<List<UserDTO>>(result)); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

    }
}
