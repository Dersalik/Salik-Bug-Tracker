using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class BugDeveloperRepository : Repository<BugDeveloper>,IBugDeveloperRepository
    {
        public BugDeveloperRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
