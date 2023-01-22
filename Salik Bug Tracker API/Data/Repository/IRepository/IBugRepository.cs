using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IBugRepository:IRepository<Bug>
    {
       Task<IEnumerable<Bug>> GetBugsByDeveloperId(string developerId);
        Task<IEnumerable<Bug>> GetBugsByModuleIdAndDeveloperId(int ModuleId, string developerId);
    }

}
