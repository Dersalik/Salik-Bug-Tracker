using Salik_Bug_Tracker_API.Data.Repository.Services;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IProjectRepository:IRepository<Project>
    {
        Task<bool> CheckProjectExists(int Id);
        Task<(IEnumerable<Project>, PaginationMetadata)> GetProjectsAsync(
            string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<IEnumerable<Module>> getModulesOfProject(int ProjectId);
        Task<Module> getParticularModuleOfProject(int ProjectId, int ModuleId);
        Task AddNewModuleToProject(int ProjectId, Module module);
        Task<Project> GetProjectWithModules(int Id);

    }
}
