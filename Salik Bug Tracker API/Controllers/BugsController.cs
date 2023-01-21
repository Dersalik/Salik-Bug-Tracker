﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;

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
        public async Task<ActionResult<List<BugDTO>>> GetBugs(int ModuleId)
        {
            try
            {
                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

                if (!IsModuleAvailable)
                {
                    return NotFound();
                }
                var bugs = await _unitOfWork.bugRepository
                .Where(b => b.ModulesId == ModuleId);


                var bugDTOs = Mapper.Map<List<BugDTO>>(bugs);

                return Ok(bugDTOs);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        [HttpGet("{BugId}",Name = "GetBug")]
        public async Task<ActionResult<BugDTO>> GetBug(int BugId, int ProjectId,  int ModuleId)
        {
            try {var bug = await _unitOfWork.bugRepository
            .GetFirstOrDefault(b=>b.Id==BugId );

            if(bug == null)
            {
                return NotFound("Bug is not available ");
            }

            var bugDTO = Mapper.Map<BugDTO>(bug);

            return Ok(bugDTO);
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }


        }

        [HttpPost]
        public async Task<ActionResult<BugDTO>> CreateBug(int ProjectId, int ModuleId,[FromBody] BugDTOForCreation bugDTO)
        {
            try
            {
                bool IsModuleAvailable = await _unitOfWork.moduleRepository.CheckModuleExists(ModuleId);

                if (!IsModuleAvailable)
                {
                    return NotFound();
                }
                bool DevExists = await _unitOfWork.userRepository.CheckDevExists(bugDTO.ApplicationUserId);

                if (!DevExists) { return NotFound("Developer doesnt exist"); }

                var bug = Mapper.Map<Bug>(bugDTO);
                bug.ModulesId = ModuleId;
                await _unitOfWork.bugRepository.Add(bug);
                await _unitOfWork.Save();

                var bugDTOToReturn = Mapper.Map<BugDTO>(bug);

                return CreatedAtRoute(nameof(GetBug), new { ProjectId = ProjectId, ModuleId = ModuleId, BugId = bug.Id }, bugDTOToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data to the database");
            }
        }

    }
}
