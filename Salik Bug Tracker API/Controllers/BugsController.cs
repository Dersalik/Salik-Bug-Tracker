using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/v{version:apiVersion}/Projects/{ProjectId}/Modules/{ModuleId}/Bugs")]
    [ApiVersion("1.0")]
    [ApiController]
    public class BugsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        private readonly ILogger<BugsController> _logger;

        public BugsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<BugsController> logger)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<BugDTO>>> GetBugs(int ModuleId)
        {
            try
            {
                _logger.LogInformation("Received request to retrieve bugs for module {ModuleId}", ModuleId);

                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

                if (!IsModuleAvailable)
                {
                    _logger.LogWarning($"Module with id {ModuleId} was not found");
                    return NotFound();
                }
                var bugs = await _unitOfWork.bugRepository
                .Where(b => b.ModulesId == ModuleId);


                var bugDTOs = Mapper.Map<List<BugDTO>>(bugs);
                _logger.LogInformation($"The bugs for module with id {ModuleId} was returned ");
                return Ok(bugDTOs);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while trying to recover bugs of module with id {ModuleId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        [HttpGet("{BugId}",Name = "GetBug")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<BugDTO>> GetBug(int BugId, int ProjectId,  int ModuleId)
        {
            try {
                _logger.LogInformation("Received request to retrieve bug {BugId} from module {ModuleId} and project {ProjectId}", BugId, ModuleId, ProjectId);

                var bug = await _unitOfWork.bugRepository
            .GetFirstOrDefault(b=>b.Id==BugId );

            if(bug == null)
            {
                    _logger.LogWarning($"Bug with id: {BugId} from project id: {ProjectId} and module id: {ModuleId} not found.");
                    return NotFound("Bug is not available ");
            }

            var bugDTO = Mapper.Map<BugDTO>(bug);
                _logger.LogInformation($"Successfully retrieved bug with id: {BugId} from project id: {ProjectId} and module id: {ModuleId}");
                return Ok(bugDTO);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error retrieving bug with id: {BugId} from project id: {ProjectId} and module id: {ModuleId}. Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }


        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<BugDTO>> CreateBug(int ProjectId, int ModuleId,[FromBody] BugDTOForCreation bugDTO)
        {
            try
            {
                _logger.LogInformation("Received request to create bug in module {ModuleId} and project {ProjectId}", ModuleId, ProjectId);

                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

                if (!IsModuleAvailable)
                {
                    _logger.LogWarning($"Module with id: {ModuleId} not found.");
                    return NotFound();
                }
                bool DevExists = await _unitOfWork.userRepository.CheckDevExists(bugDTO.ApplicationUserId);

                if (!DevExists) {
                    _logger.LogWarning($"Developer with id: {bugDTO.ApplicationUserId} not found.");
                    return NotFound("Developer doesnt exist"); }

                var bug = Mapper.Map<Bug>(bugDTO);
                bug.ModulesId = ModuleId;
                await _unitOfWork.bugRepository.Add(bug);
                await _unitOfWork.Save();

                var bugDTOToReturn = Mapper.Map<BugDTO>(bug);
                _logger.LogInformation($"Successfully created bug with id: {bug.Id} for project id: {ProjectId} and module id: {ModuleId}");
                return CreatedAtRoute(nameof(GetBug), new { ProjectId = ProjectId, ModuleId = ModuleId, BugId = bug.Id }, bugDTOToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating bug for project id: {ProjectId} and module id: {ModuleId}. Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data to the database");
            }
        }
        [HttpPut("{BugId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateBug(int BugId, int ProjectId, int ModuleId, BugDTOForUpdate bugDTO)
        {
            try
            {
                _logger.LogInformation("Received request to update bug {BugId} in module {ModuleId} and project {ProjectId}", BugId, ModuleId, ProjectId);

                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);
                if (!IsModuleAvailable)
                {
                    _logger.LogWarning($"Module with id: {ModuleId} not found.");
                    return NotFound();
                }
                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                if (bug == null)
                {
                    _logger.LogWarning($"Bug with id: {BugId} not found.");
                    return NotFound("Bug not found");
                }
                Mapper.Map(bugDTO, bug);
                _unitOfWork.bugRepository.UpdateEntity(bug);
                await _unitOfWork.Save();
                _logger.LogInformation($"Successfully updated bug with id: {BugId} for project id: {ProjectId} and module id: {ModuleId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bug with id: {BugId} for project id: {ProjectId} and module id: {ModuleId}. Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating bug in the database");
            }
        }
        [HttpPatch("{BugId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateBug(int BugId, int ProjectId, int ModuleId, JsonPatchDocument<BugDTOForUpdate> patchDoc)
        {
            try
            {
                _logger.LogInformation("Received request to partially update bug {BugId} in module {ModuleId} and project {ProjectId}", BugId, ModuleId, ProjectId);

                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);
                if (!IsModuleAvailable)
                {
                    _logger.LogWarning($"Module with id: {ModuleId} not found.");
                    return NotFound();
                }
                var bug = await _unitOfWork.bugRepository.GetFirstOrDefault(b => b.Id == BugId);
                if (bug == null)
                {
                    _logger.LogWarning($"Bug with id: {BugId} not found.");
                    return NotFound("Bug not found");
                }
                var bugDTO = Mapper.Map<BugDTOForUpdate>(bug);
                patchDoc.ApplyTo(bugDTO, ModelState);
                TryValidateModel(bugDTO);
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state while patching bug with id: {BugId} for project id: {ProjectId} and module id: {ModuleId}.");
                    return BadRequest(ModelState);
                }
                Mapper.Map(bugDTO, bug);
                _unitOfWork.bugRepository.UpdateEntity(bug);
                await _unitOfWork.Save(); 
                _logger.LogInformation($"Successfully patched bug with id: {BugId} for project id: {ProjectId} and module id: {ModuleId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error patching bug with id: {BugId} for project id: {ProjectId} and module id: {ModuleId}. Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating bug in the database");
            }
        }
    }
}
