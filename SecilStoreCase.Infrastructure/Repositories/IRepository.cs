using System.Linq.Expressions;

namespace SecilStoreCase.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate); 
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null); 
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
