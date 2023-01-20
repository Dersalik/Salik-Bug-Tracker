using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Salik_Bug_Tracker_API.Controllers
{
 
    [Route("api/Users/{UserId}/Skills")]
    [ApiController]
    public class SkillsController : ControllerBase
    {

        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }

        public SkillsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<SkillDTO>>> getSkills(string UserId)
        {
            try
            {
                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);

                return Ok(Mapper.Map<List<SkillDTO>>(DevWithSkills.skills));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        [HttpGet("{skillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<SkillDTO>>> getSkill(string UserId, int skillId)
        {
            try
            {
                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == skillId);
                if (!SkillExists) { return NotFound("Skill doesnt exist"); }
                return Ok(Mapper.Map<SkillDTO>(DevWithSkills.skills.FirstOrDefault(d => d.Id == skillId)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddSkill(SkillDTOForCreation Skill, string UserId)
        {
            try
            {
                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

         
                if (!IsDevAvailable)
                {
                    return NotFound("Developer doesn't exist");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please, provide all the required fields");
                }
                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                var skillToAdd = Mapper.Map<Skill>(Skill);
                DevWithSkills.skills.Add(skillToAdd);
                await _unitOfWork.Save();
                return CreatedAtAction(nameof(getSkill), new { UserId = UserId, skillId = skillToAdd.Id }, Mapper.Map<SkillDTO>(skillToAdd));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data to the database");
            }
        }


        [HttpDelete("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> deleteSkill(string UserId,int SkillId)
        {
            try
            {
                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == SkillId);
                if (!SkillExists) { return NotFound("Skill doesnt exist"); }
                _unitOfWork.skillRepository.Remove(DevWithSkills.skills.FirstOrDefault(d => d.Id == SkillId));
                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }
        [HttpPut("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> EditSkill(int SkillId, SkillDTOForUpdate Skill, string UserId)
        {
            try
            {
                var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

                if (!IsDevAvailable)
                {
                    return NotFound("Developer doesn't exist");
                }

                var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == SkillId);
                if (!SkillExists) { return NotFound("Skill doesnt exist"); }

                var skillFromRepo = DevWithSkills.skills.FirstOrDefault(d => d.Id == SkillId);
                Mapper.Map(Skill, skillFromRepo);

                _unitOfWork.skillRepository.UpdateEntity(skillFromRepo);
                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }





    }
}
