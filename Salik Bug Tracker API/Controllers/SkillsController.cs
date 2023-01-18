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
        public async Task<ActionResult<List<Skill>>> getSkills(string UserId)
        {
            var IsDevAvailable =await _unitOfWork.userRepository.CheckDevExists(UserId);

            if (!IsDevAvailable)
            {
                return NotFound("Developer doesn't exist");
            }

            var DevWithSkills=await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d=>d.Id== UserId);

            return Ok(Mapper.Map<List<SkillDTO>>(DevWithSkills.skills));
        }
        [HttpPost]
        public async Task<ActionResult> AddSkill(SkillDTO Skill,string UserId)
        {
            var IsDevAvailable = await _unitOfWork.userRepository.CheckDevExists(UserId);

            if (!IsDevAvailable)
            {
                return NotFound("Developer doesn't exist");
            }

            var DevWithSkills = await _unitOfWork.userRepository.GetFirstOrDefaultWithSkills(d => d.Id == UserId);
             DevWithSkills.skills.Add(new Skill { Level = Skill.Level, Name = Skill.Name });
            await _unitOfWork.Save();
            return Ok();
        }
    }
}
