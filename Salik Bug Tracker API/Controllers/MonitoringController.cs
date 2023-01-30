
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Salik_Bug_Tracker_API.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MonitoringController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MonitoringController(ApplicationDbContext db)
        {
            _db = db;
        }
        /// <summary>
        /// gets the current status of the database 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new
            {
                CanConnectToDatabase = _db.Database.CanConnect()
            });
        }

      

      
    }
}
