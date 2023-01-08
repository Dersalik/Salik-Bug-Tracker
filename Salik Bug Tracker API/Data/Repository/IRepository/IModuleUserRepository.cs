using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IModuleUserRepository:IRepository<ModuleUser>
    {
        Task<bool> checkIfModuleAlreadyAssignedToDev(int moduleId, string DevId);

    }
}
