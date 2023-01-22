using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class BugDeveloperRepository : Repository<BugDeveloper>,IBugDeveloperRepository
    {
        public BugDeveloperRepository(ApplicationDbContext db) : base(db)
        {
            _db= db;

        }
        private ApplicationDbContext _db;

        public async Task<IEnumerable<ApplicationUser>> GetDevelopersByBugId(int bugId)
        {
            var developers = await _db.bugDevelopers
                            .Where(ba => ba.BugId == bugId)
                            .Select(ba => ba.user)
                            .ToListAsync();
            return developers;
        }
    }
}
