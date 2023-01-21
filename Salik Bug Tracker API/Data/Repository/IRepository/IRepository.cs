using System.Linq.Expressions;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        Task Add(T entity);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAll();
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void UpdateEntity(T entity);
        Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate);

    }
}
