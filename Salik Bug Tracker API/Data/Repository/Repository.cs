using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using System.Linq.Expressions;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();

        }
        public async void Add(T entity)
        {

            await dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            

            return await dbSet.ToListAsync();
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {

            dbSet.RemoveRange(entity);
        }

        public void UpdateEntity(T entity)
        {

            _db.Update(entity);
        }
    }
}
