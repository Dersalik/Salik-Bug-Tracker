using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class BugRepository : Repository<Bug>, IBugRepository
    {
        public BugRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
