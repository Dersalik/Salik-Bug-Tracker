using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
