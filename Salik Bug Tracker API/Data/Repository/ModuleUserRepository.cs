using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Data;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class ModuleUserRepository : Repository<ModuleUser>, IModuleUserRepository
    {
        public ModuleUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        private ApplicationDbContext _db;


        public async Task<bool> checkIfModuleAlreadyAssignedToDev(int moduleId, string DevId)
        {
           return await _db.ModuleUsers.AnyAsync(d => d.ModuleId == moduleId && d.ApplicationUserId == DevId);
        }
    }
}

