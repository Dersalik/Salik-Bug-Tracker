using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/[controller]/Projects/{ProjectId}/Modules")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }

        public ModulesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetModules(int ProjectId)
        {
             
            bool IsProjectAvailable=await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if(!IsProjectAvailable)
            {
               return NotFound();
            }

            var modulesOfProject=await _unitOfWork.projectRepository.getModulesOfProject(ProjectId);

            return Ok(Mapper.Map<ModuleDTO>(modulesOfProject));
        }

        [HttpGet("{ModuleId}",Name = "GetModule")]
        public async Task<ActionResult<ModuleDTO>> GetModule(int ProjectId, int ModuleId)
        {
            bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }

            var Module = _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if(Module == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<ModuleDTO>(Module));
        }

        [HttpPost]  
        public async Task<ActionResult<ModuleDTO>> CreateModule(int ProjectId,ModuleForCreationDTO Module)
        {
            bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }

            var ModuleToAdd = Mapper.Map<Module>(Module);

            await _unitOfWork.projectRepository.AddNewModuleToProject(ProjectId, ModuleToAdd);
            await _unitOfWork.Save();

            var CreatedModuleToReturn=Mapper.Map<ModuleDTO>(ModuleToAdd);

            return CreatedAtRoute("GetModule", new { ProjectId = ProjectId, ModuleId = CreatedModuleToReturn.Id },CreatedModuleToReturn);
        }

    }
}
