using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }

        public UsersController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("{DeveloperId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> getDeveloper(string DeveloperId)
        {
            var DevExists = await _unitOfWork.userRepository.CheckDevExists(DeveloperId);

            if (!DevExists)
            {
                return NotFound(DeveloperId);
            }
            var result = await _unitOfWork.userRepository.GetFirstOrDefault(d => d.Id == DeveloperId);
            return Ok(Mapper.Map<UserDTO>(result));
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDTO>>> getDeveloper()
        {

           
            var result = await _unitOfWork.userRepository.GetAll();
            return Ok(Mapper.Map<List<UserDTO>>(result));
        }
    }
}
