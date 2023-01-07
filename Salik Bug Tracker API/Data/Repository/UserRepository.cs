using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;
using System.Linq.Expressions;

namespace Salik_Bug_Tracker_API.Data.Repository
{
   

    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db= db;
        }
        private ApplicationDbContext _db;

        public async Task<ApplicationUser> GetFirstOrDefaultWithAllAttributes (Expression<Func<ApplicationUser, bool>> filter)
        {

            return await _db.applicationUsers.Include(d => d.bugDevelopers).Include(d => d.moduleUsers).Include(d => d.skills).FirstOrDefaultAsync(filter);
        }
        public async Task<bool> CheckDevExists(string Id)
        {
            return await _db.applicationUsers.AnyAsync(d => d.Id == Id);
        }

        public async Task<IEnumerable<ApplicationUser>> getDevsOfOneModule(int moduleId)
        {
           var result= await _db.ModuleUsers.FirstOrDefaultAsync(d=> d.Id == moduleId);  
            return await _db.applicationUsers.Where(d => d.moduleUsers.Contains(result)).ToListAsync();
        }
    }
}
