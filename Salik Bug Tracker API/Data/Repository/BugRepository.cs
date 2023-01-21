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

       
    }
}
