using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        public ProjectController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.Mapper= mapper;    
            this._unitOfWork= unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<List<ProjectDTO>>> getProjects()
        {
           var result= await _unitOfWork.projectRepository.GetAll();

            return Ok(Mapper.Map<List<ProjectDTO>>(result));
        }
    }
}
