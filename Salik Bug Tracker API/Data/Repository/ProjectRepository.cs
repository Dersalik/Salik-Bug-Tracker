using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Data.Repository.Services;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
   
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private ApplicationDbContext _db;
        public ProjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> CheckProjectExists(int Id)
        {
            return await _db.projects.AnyAsync(d=>d.Id==Id);
        }

        public async Task<(IEnumerable<Project>, PaginationMetadata)> GetProjectsAsync(
            string? name,string? searchQuery,int pageNumber,int pageSize)
        {
            var collection=_db.projects.AsQueryable<Project>();

            if(!string.IsNullOrWhiteSpace(name))
            {
                name= name.Trim();
                collection=collection.Where(d=>d.Name==name);
            }

            if(!string.IsNullOrWhiteSpace(searchQuery)) {
                searchQuery= searchQuery.Trim();
                collection=collection.Where(a=>a.Name.Contains(searchQuery) || (a.Description !=null && a.Description.Contains(searchQuery)) );

            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata=new PaginationMetadata(totalItemCount, pageSize,pageNumber);

            var collectionsToReturn=await collection.OrderBy(d=>d.Name)
                .Skip(pageSize * (pageNumber -1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionsToReturn,paginationMetadata);

        }
    }
}
