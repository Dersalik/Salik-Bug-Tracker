using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/Projects/{ProjectId}/Modules/{ModuleId}/Bugs")]
    [ApiController]
    public class BugsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }

        public BugsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<List<BugDTO>>> GetBugs( int ModuleId)
        {
            var bugs = await _unitOfWork.bugRepository
                .Where(b => b.ModulesId == ModuleId);
                

            var bugDTOs = Mapper.Map<List<BugDTO>>(bugs);

            return Ok(bugDTOs);
        }



    }
}
