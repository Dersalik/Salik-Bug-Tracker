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
            try {
                _logger.LogInformation("Received request to retrieve modules for project {ProjectId}", ProjectId);
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
            }

            var modulesOfProject = await _unitOfWork.projectRepository.getModulesOfProject(ProjectId);
                _logger.LogInformation($"GetModules called successfully for project with ID {ProjectId}");
                return Ok(Mapper.Map<IEnumerable<ModuleDTO>>(modulesOfProject)); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving modules for project with ID {ProjectId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

        [HttpGet("{ModuleId}", Name = "GetModule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDTO>> GetModule(int ProjectId, int ModuleId)
        {
            try {
                _logger.LogInformation("Received request to retrieve module {ModuleId} from project {ProjectId}", ModuleId, ProjectId);
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning($"Project with ID {ProjectId} not found");
                    return NotFound();
            }

            var Module = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (Module == null)
            {
                    _logger.LogWarning($"Module with ID {ModuleId} not found for project with ID {ProjectId}");
                    return NotFound();
            }
            var result = Mapper.Map<ModuleDTO>(Module);
                _logger.LogInformation($"GetModule called successfully for project with ID {ProjectId} and module with ID {ModuleId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving module with ID {ModuleId} for project with ID {ProjectId}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data in the database");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDTO>> CreateModule(int ProjectId, [FromBody] ModuleForCreationDTO Module)
        {
            try {
                _logger.LogInformation("Received request to create module in project {ProjectId} with name {ModuleName}", ProjectId, Module.Name);
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning("Project with ID {ProjectId} not found", ProjectId);
                    return NotFound();
            }
            if (!ModelState.IsValid)
            {
                    _logger.LogWarning("Invalid model state while creating module");
                    return BadRequest("Please, provide all the required fields");
            }
            var ModuleToAdd = Mapper.Map<Module>(Module);

            await _unitOfWork.projectRepository.AddNewModuleToProject(ProjectId, ModuleToAdd);
            await _unitOfWork.Save();

            var CreatedModuleToReturn=Mapper.Map<ModuleDTO>(ModuleToAdd);
                _logger.LogInformation("Successfully created a new module with id {ModuleId} for project {ProjectId}.", CreatedModuleToReturn.Id, ProjectId);
                return CreatedAtRoute("GetModule", new { ProjectId = ProjectId, ModuleId = CreatedModuleToReturn.Id },CreatedModuleToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting data in the database");
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
                _logger.LogInformation($"Updating Module with ProjectId: {ProjectId} and ModuleId: {ModuleId} using data from request body.");
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

                if (!IsProjectAvailable)
                {
                    _logger.LogError($"Project with id: {ProjectId} was not found when trying to update module with id: {ModuleId}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while updating module");
                    return BadRequest("Please, provide all the required fields");
                }
                var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

                if (ModuleEntity == null)
                {
                    _logger.LogError($"Module with id: {ModuleId} was not found when trying to update module for project with id: {ProjectId}");
                    return NotFound();
                }

                Mapper.Map(module, ModuleEntity);
                await _unitOfWork.Save();
                _logger.LogInformation($"Successfully updated module with id: {ModuleId} for project with id: {ProjectId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to update module with id: {ModuleId} for project with id: {ProjectId}. Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }

        [HttpPatch("{ModuleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PartiallyUpdateModule(int ProjectId,int ModuleId, [FromBody] JsonPatchDocument<ModuleForUpdateDTO> patchDocument)
        {
            try {
                _logger.LogInformation($"Partially updating Module with ID: { ModuleId} in Project with ID: { ProjectId}.");
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning("Project with id {ProjectId} not found", ProjectId);
                    return NotFound();
            }
            if (!ModelState.IsValid)
            {
                    _logger.LogWarning("Invalid model state for patching module with id {ModuleId} in project with id {ProjectId}", ModuleId, ProjectId);
                    return BadRequest("Please, provide all the required fields");
            }
            var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (ModuleEntity == null)
            {
                    _logger.LogWarning("Module with id {ModuleId} not found in project with id {ProjectId}", ModuleId, ProjectId);
                    return NotFound();
            }

            var moduleToPacth=Mapper.Map<ModuleForUpdateDTO>(ModuleEntity);

            patchDocument.ApplyTo(moduleToPacth,ModelState);

            if(!ModelState.IsValid) { 
            return BadRequest(ModelState);
            }

            if (!TryValidateModel(moduleToPacth))
            {
                    _logger.LogWarning("Invalid model state for patching module with id {ModuleId} in project with id {ProjectId}", ModuleId, ProjectId);
                    return BadRequest(ModelState);
            }
            Mapper.Map(moduleToPacth, ModuleEntity);
            await _unitOfWork.Save();
                _logger.LogInformation("Successfully patched module with id {ModuleId} in project with id {ProjectId}", ModuleId, ProjectId);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating module with id {ModuleId} in project with id {ProjectId}. Error: {ErrorMessage}", ModuleId, ProjectId, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }

        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteModule(int ProjectId,int ModuleId)
        {
            try {
                _logger.LogInformation($"Deleting Module with ID: {ModuleId} from Project with ID: {ProjectId}.");
                bool IsProjectAvailable = await _unitOfWork.projectRepository.CheckProjectExists(ProjectId);

            if (!IsProjectAvailable)
            {
                    _logger.LogWarning($"Project with id: {ProjectId} not found");
                    return NotFound();
            }

            var ModuleEntity = await _unitOfWork.projectRepository.getParticularModuleOfProject(ProjectId, ModuleId);

            if (ModuleEntity == null)
                {
                    _logger.LogWarning($"Module with id: {ModuleId} not found for project with id: {ProjectId}");
                    return NotFound();
            }

            _unitOfWork.moduleRepository.Remove(ModuleEntity);
            await _unitOfWork.Save(); 
              _logger.LogInformation($"Successfully deleted module with id: {ModuleId} for project with id: {ProjectId}");
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting data in the database module id {ModuleId} that belongs to {ProjectId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data in the database");
            }

        }
    }
}
