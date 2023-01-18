using Salik_Bug_Tracker_API.Models;
using System.Linq.Expressions;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IUserRepository:IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetFirstOrDefaultWithAllAttributes(Expression<Func<ApplicationUser, bool>> filter);
        Task<bool> CheckDevExists(string Id);
        Task<IEnumerable<ApplicationUser>> getDevsOfOneModule(int moduleId);
        Task<ApplicationUser> GetFirstOrDefaultWithSkills(Expression<Func<ApplicationUser, bool>> filter);

    }
}
