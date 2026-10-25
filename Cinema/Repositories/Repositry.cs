using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema.Repositories
{
    public class Repositry<T> : IRepositry<T> where T : class
    {
        protected readonly ApplicationDbContext _context;// = new ApplicationDbContext();

        private DbSet<T> _dbSet;

        public Repositry(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = _dbSet.AsQueryable();
            if (filter != null)
                entities = entities.Where(filter);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!isTracked)
            {
                entities = entities.AsNoTracking();
            }

            return entities;
        }
        public async Task<IEnumerable<T>> GetListAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.ToListAsync();
        }

        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.FirstOrDefaultAsync();
        }

        public void Update(T entites)
        {
            _dbSet.Update(entites);
        }

        public void Remove(T entites)
        {
            _dbSet.Remove(entites);
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? filter = null
            )
        {
            if (filter == null)
                return await _dbSet.CountAsync();

            return await _dbSet.CountAsync(filter);
        }
    }
}
