using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema.Repositories
{
    public interface IRepositry<T> where T:class
    {
        Task<EntityEntry<T>> AddAsync(T entity);

        IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );
        Task<IEnumerable<T>> GetListAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );

        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );

        void Update(T entites);

        void Remove(T entites);

        Task<int> CommitAsync();

        Task<int> CountAsync(
            Expression<Func<T, bool>>? filter = null
            );
    }
}
