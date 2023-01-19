using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

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
            var IsDevAvailable =await _unitOfWork.userRepository.CheckDevExists(UserId);

            if (!IsDevAvailable)
            {
                return NotFound("Developer doesn't exist");
            }

            var DevWithSkills=await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d=>d.Id== UserId);

            return Ok(Mapper.Map<List<SkillDTO>>(DevWithSkills.skills));
        }
        [HttpGet("{skillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<SkillDTO>>> getSkill(string UserId,int skillId)
        {
            var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

            if (!IsDevAvailable)
            {
                return NotFound("Developer doesn't exist");
            }

            var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
            bool SkillExists = DevWithSkills.skills.Exists(d => d.Id == skillId);
            if (!SkillExists) { return NotFound("Skill doesnt exist"); }
            return Ok(Mapper.Map<SkillDTO>(DevWithSkills.skills.FirstOrDefault(d=>d.Id==skillId)));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddSkill(SkillDTOForCreation Skill,string UserId)
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
            return CreatedAtAction(nameof(getSkill), new { UserId = UserId, skillId= skillToAdd.Id}, Mapper.Map<SkillDTO>(skillToAdd));

        }

        [HttpDelete("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> deleteSkill(string UserId,int SkillId)
        {
            var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

            if (!IsDevAvailable)
            {
                return NotFound("Developer doesn't exist");
            }
            var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
            bool SkillExists= DevWithSkills.skills.Exists(d=>d.Id== SkillId);
            if (!SkillExists) { return NotFound("Skill doesnt exist"); }
           var skillToDelete= await _unitOfWork.skillRepository.GetFirstOrDefault(d => d.Id== SkillId);

            _unitOfWork.skillRepository.Remove(skillToDelete);
            await _unitOfWork.Save();
            return NoContent();

        }
        [HttpPut("{SkillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> EditSkill(string UserId, int SkillId, SkillDTOForUpdate skill)
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
                var devWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
                bool skillExists = devWithSkills.skills.Exists(d => d.Id == SkillId);
                if (!skillExists) { return NotFound("Skill doesnt exist"); }
                var skillToEdit = await _unitOfWork.skillRepository.GetFirstOrDefault(d => d.Id == SkillId);
                Mapper.Map(skill, skillToEdit);
                await _unitOfWork.Save();
                return Ok();
            
            
        }
    }
}
