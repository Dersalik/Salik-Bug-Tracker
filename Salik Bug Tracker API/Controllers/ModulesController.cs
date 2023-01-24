using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using System.Text.Json;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/v{version:apiVersion}/Projects/{ProjectId}/Modules")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        private readonly ILogger<ModulesController> _logger;

        public ModulesController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ModulesController> logger)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetModules(int ProjectId)
        {
            try {  bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }

            var modulesOfProject = await _unitOfWork.projectRepository.getModulesOfProject(ProjectId);

            return Ok(Mapper.Map<IEnumerable<ModuleDTO>>(modulesOfProject)); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

        [HttpGet("{ModuleId}", Name = "GetModule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDTO>> GetModule(int ProjectId, int ModuleId)
        {
            try { bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }

            var Module = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (Module == null)
            {
                return NotFound();
            }
            var result = Mapper.Map<ModuleDTO>(Module);
            return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDTO>> CreateModule(int ProjectId, [FromBody] ModuleForCreationDTO Module)
        {
            try {bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all the required fields");
            }
            var ModuleToAdd = Mapper.Map<Module>(Module);

            await _unitOfWork.projectRepository.AddNewModuleToProject(ProjectId, ModuleToAdd);
            await _unitOfWork.Save();

            var CreatedModuleToReturn=Mapper.Map<ModuleDTO>(ModuleToAdd);

            return CreatedAtRoute("GetModule", new { ProjectId = ProjectId, ModuleId = CreatedModuleToReturn.Id },CreatedModuleToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting data in the database");
            }
        }

        [HttpPut("{ModuleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateModule(int ProjectId,int ModuleId, [FromBody] ModuleForUpdateDTO module)
        {
            try
            {

                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

                if (!IsProjectAvailable)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please, provide all the required fields");
                }
                var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

                if (ModuleEntity == null)
                {
                    return NotFound();
                }

                Mapper.Map(module, ModuleEntity);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }

        [HttpPatch("{ModuleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PartiallyUpdateModule(int ProjectId,int ModuleId, [FromBody] JsonPatchDocument<ModuleForUpdateDTO> patchDocument)
        {
            try {bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all the required fields");
            }
            var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (ModuleEntity == null)
            {
                return NotFound();
            }

            var moduleToPacth=Mapper.Map<ModuleForUpdateDTO>(ModuleEntity);

            patchDocument.ApplyTo(moduleToPacth,ModelState);

            if(!ModelState.IsValid) { 
            return BadRequest(ModelState);
            }

            if (!TryValidateModel(moduleToPacth))
            {
                return BadRequest(ModelState);
            }
            Mapper.Map(moduleToPacth, ModuleEntity);
            await _unitOfWork.Save();
            
            return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }

        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteModule(int ProjectId,int ModuleId)
        {
            try {bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                return NotFound();
            }

            var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (ModuleEntity == null)
            {
                return NotFound();
            }

            _unitOfWork.moduleRepository.Remove(ModuleEntity);
            await _unitOfWork.Save();
            return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data in the database");
            }

        }
    }
}
