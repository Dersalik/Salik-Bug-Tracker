using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class ModuleUserRepository : Repository<ModuleUser>, IModuleUserRepository
    {
        public ModuleUserRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
