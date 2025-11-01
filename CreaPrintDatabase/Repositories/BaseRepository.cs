using CreaPrintCore.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CreaPrintCore.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            // if entity supports audit, set CreatedOn
            if (entity is AuditableEntity aud)
            {
                aud.CreatedOn = DateTime.UtcNow;
            }

            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            // if entity supports audit, set UpdatedOn
            if (entity is AuditableEntity aud)
            {
                aud.UpdatedOn = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        // New helper: update and return the refreshed entity from the database
        public virtual async Task<T?> UpdateAndGetAsync(T entity)
        {
            // if entity supports audit, set UpdatedOn
            if (entity is AuditableEntity aud)
            {
                aud.UpdatedOn = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            // Reload the entity from the database to get current values (including related data if available)
            var refreshed = await GetByIdAsync(entity.Id);
            return refreshed;
        }

        // Generic find by predicate (use Expression to allow EF translation)
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) return Enumerable.Empty<T>();
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet.Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<int> GetCountAsync(Func<T, bool>? filter = null)
        {
            if (filter == null)
                return await _dbSet.CountAsync();
            else
                return await Task.FromResult(_dbSet.Count(filter));
        }
    }
}
