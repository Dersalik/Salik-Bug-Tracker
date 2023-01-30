using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Salik_Bug_Tracker_API.Controllers
{
 
    [Route("api/v{version:apiVersion}/Users/{UserId}/Skills")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SkillsController : ControllerBase
    {

        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        private readonly ILogger<SkillsController> _logger;

        public SkillsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<SkillsController> logger)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        /// <summary>
        /// gets all of the skills that a developer has
        /// </summary>
        /// <param name="UserId"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<SkillDTO>>> getSkills(string UserId)
        {
            try
            {
                _logger.LogInformation($"getting skills of developer with id {UserId}");

                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    _logger.LogError("Developer not found");
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                _logger.LogInformation($"Retrieved skills for developer with id {UserId}");

                return Ok(Mapper.Map<List<SkillDTO>>(DevWithSkills.skills));
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occurred while retrieving skills for developer with id {UserId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        /// <summary>
        /// Gets a particular skill 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="skillId"></param>
        [HttpGet("{skillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<SkillDTO>>> getSkill(string UserId, int skillId)
        {
            try
            {
                _logger.LogInformation($"getting skill with id {skillId} of developer with id {UserId}");

                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    _logger.LogWarning($"Developer with id {UserId} was not found");
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == skillId);
                if (!SkillExists) {
                    _logger.LogWarning($"Skill with id {skillId} was not found");
                    return NotFound("Skill doesnt exist"); 
                 
                }
                _logger.LogInformation($"Retrieved skill with id {skillId} for developer with id {UserId}");
                return Ok(Mapper.Map<SkillDTO>(DevWithSkills.skills.FirstOrDefault(d => d.Id == skillId)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving skill with id {skillId} for developer with id {UserId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        /// <summary>
        /// Adds a new skill
        /// </summary>
        /// <param name="Skill"></param>
        /// <param name="UserId"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> AddSkill([FromBody] SkillDTOForCreation Skill, string UserId)
        {
            try
            {
                _logger.LogInformation($"adding a new skill for developer with id {UserId}");

                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

         
                if (!IsDevAvailable)
                {
                    _logger.LogWarning($"Dev doesnt exists with id : {UserId}");
                    return NotFound("Developer doesn't exist");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Please provide all the required fields along with the specific requrements for each");
                    return BadRequest("Please, provide all the required fields");
                }
                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                var skillToAdd = Mapper.Map<Skill>(Skill);
                DevWithSkills.skills.Add(skillToAdd);
                await _unitOfWork.Save();
                _logger.LogInformation($"New skill with id {skillToAdd.Id} was add for developer with id {UserId}");
                return CreatedAtAction(nameof(getSkill), new { UserId = UserId, skillId = skillToAdd.Id }, Mapper.Map<SkillDTO>(skillToAdd));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding a new skill for developer with id {UserId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data to the database");
            }
        }

        /// <summary>
        /// deletes a skill 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="SkillId"></param>
        [HttpDelete("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> deleteSkill(string UserId,int SkillId)
        {
            try
            {
                _logger.LogInformation($"deleting skill with id {SkillId} for developer with id {UserId}");

                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    _logger.LogWarning($"failed to find developer with id {UserId}");
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == SkillId);
                if (!SkillExists) {
                    _logger.LogWarning($"failed to find skill with id {SkillId} for developer with id {UserId}");
                    return NotFound("Skill doesnt exist");
                }
                _unitOfWork.skillRepository.Remove(DevWithSkills.skills.FirstOrDefault(d => d.Id == SkillId));
                await _unitOfWork.Save();
                _logger.LogInformation($"skill with id {SkillId} that belongs to developer with id {UserId} was successfuly deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while trying to delete a skill with id {SkillId} for developer with id {UserId} : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }
        /// <summary>
        /// edits a skill 
        /// </summary>
        /// <param name="SkillId"></param>
        /// <param name="Skill"></param>
        /// <param name="UserId"></param>
        [HttpPut("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> EditSkill(int SkillId, [FromBody] SkillDTOForUpdate Skill, string UserId)
        {
            try
            {
                _logger.LogInformation($"edit skill with id {SkillId} for developer with id {UserId}");

                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    _logger.LogWarning($"Developer with id {UserId} was not found");
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills!.skills!.Exists(d => d.Id == SkillId);
                if (!SkillExists) {
                    _logger.LogWarning($"failed to find skill with id {SkillId} for developer with id {UserId}");
                    return NotFound("Skill doesnt exist");
                }

                var skillFromRepo = DevWithSkills.skills.FirstOrDefault(d => d.Id == SkillId);
                Mapper.Map(Skill, skillFromRepo);

                _unitOfWork.skillRepository.UpdateEntity(skillFromRepo!);
                await _unitOfWork.Save();
                _logger.LogInformation($"skill with id {SkillId} that belongs to developer with id {UserId} was updated successfuly ");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while trying to update a skill with id {SkillId} for developer with id {UserId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }





    }
}
