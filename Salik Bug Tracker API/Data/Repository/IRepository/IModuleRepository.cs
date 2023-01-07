
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
   
    public interface IModuleRepository : IRepository<Module>
    {
        Task<bool> CheckModuleExists(int Id);

    }
}
