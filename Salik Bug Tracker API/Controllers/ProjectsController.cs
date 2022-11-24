using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using System.Text.Json;
namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private IMapper Mapper
        {
            get;
        }
        private IUnitOfWork _unitOfWork { get; }
        const int maxProjectPageSize = 14;

        public ProjectsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.Mapper = mapper;
            this._unitOfWork = unitOfWork;
        }
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<List<ProjectDTO>>> getProjects()
        //{
        //    var result = await _unitOfWork.projectRepository.GetAll();

        //    return Ok(Mapper.Map<List<ProjectDTO>>(result));
        //}

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> getProjects([FromQuery]
            string? name, [FromQuery] string? searchQuery, [FromQuery] int pageNumber=1, [FromQuery] int pageSize=10)
        {
            if (pageSize > maxProjectPageSize)
            {
                pageSize = maxProjectPageSize;
            }

            var (projects, paginationMetadata) = await _unitOfWork.projectRepository.GetProjectsAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
            var test = Mapper.Map<IEnumerable<ProjectDTO>>(projects);
            return Ok(Mapper.Map<IEnumerable<ProjectDTO>>(projects));

        }


        [HttpGet("{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDTO>> getProject(int projectId) {
            var projectExists = await _unitOfWork.projectRepository.CheckProjectExists(projectId);

            if (!projectExists)
            {
                return NotFound(projectId);
            }
            var result=await _unitOfWork.projectRepository.GetFirstOrDefault(d=>d.Id== projectId);  
            return Ok(Mapper.Map<ProjectDTO>(result));
        }

        [HttpPut("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] ProjectForUpdateDTO projectDto)
        {
            if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId)) {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultOfMapping = Mapper.Map<Project>(projectDto);
            _unitOfWork.projectRepository.UpdateEntity(resultOfMapping);
            await _unitOfWork.Save();

            return NoContent();
        }

        [HttpPatch("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(int ProjectId, [FromBody] JsonPatchDocument<ProjectForUpdateDTO> patchDocument)
        {
            if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
            {
                return NotFound();
            }

            var project=await _unitOfWork.projectRepository.GetFirstOrDefault(d=>d.Id== ProjectId);

            var ProjectToPach=Mapper.Map<ProjectForUpdateDTO>(project);

            patchDocument.ApplyTo(ProjectToPach, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(ProjectToPach))
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(ProjectToPach,project);
             _unitOfWork.projectRepository.UpdateEntity(project);
            await _unitOfWork.Save();
            return NoContent();

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectForCreationDTO>> CreateProject([FromBody] ProjectForCreationDTO projectDto)
        {
          var result=  Mapper.Map<Project>(projectDto);
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all the required fields");
            }

            var checkResult =await _unitOfWork.projectRepository.GetFirstOrDefault(d => d.Name.Equals(result.Name));

            if (checkResult != null)
            {
                return BadRequest("A project with similar name already exists");

            }

            await _unitOfWork.projectRepository.Add(result);
            await _unitOfWork.Save();
            return CreatedAtAction(nameof(getProject),new { projectId=result.Id},Mapper.Map<ProjectDTO>(result));
        }

        [HttpDelete("{ProjectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProject(int ProjectId)
        {
            if (!await _unitOfWork.projectRepository.CheckProjectExists(ProjectId))
            {
                return NotFound();
            }

            var ProjectToDelete = await _unitOfWork.projectRepository.GetFirstOrDefault(d => d.Id==ProjectId);

             _unitOfWork.projectRepository.Remove(ProjectToDelete);
            await _unitOfWork.Save();

            return NoContent();
        }
    }
}
