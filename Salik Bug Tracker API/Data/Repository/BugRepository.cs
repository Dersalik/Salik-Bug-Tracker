using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;
using System.Linq.Expressions;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class BugRepository : Repository<Bug>, IBugRepository
    {
        private ApplicationDbContext _db;

        public BugRepository(ApplicationDbContext db) : base(db)
        {
             _db= db;
        }

        public async Task<IEnumerable<Bug>> GetBugsByDeveloperId(string developerId)
        {
            var bugs = await _db.bugs
            .Include(b => b.bugDevelopers)
            .Where(b => b.bugDevelopers.Any(bd => bd.ApplicationUserId == developerId))
            .ToListAsync();
            return bugs;
        }
        public async Task<IEnumerable<Bug>> GetBugsByModuleIdAndDeveloperId(int ModuleId, string developerId)
        {
            var bugs = await _db.bugs.Include(b => b.bugDevelopers)
.Where(b => b.ModulesId == ModuleId && b.bugDevelopers.Any(bd => bd.ApplicationUserId == developerId))
.ToListAsync();
            return bugs;
        }
    }
}
