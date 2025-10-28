using CreaPrintCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintCore.Repositories
{
 public class GenericRepository<T> : IGenericRepository<T> where T : BaseItem
 {
 protected readonly DbContext _context;
 protected readonly DbSet<T> _dbSet;

 public GenericRepository(DbContext context)
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
 _dbSet.Add(entity);
 await _context.SaveChangesAsync();
 return entity;
 }

 public virtual async Task UpdateAsync(T entity)
 {
 _dbSet.Update(entity);
 await _context.SaveChangesAsync();
 }

 public virtual async Task DeleteAsync(int id)
 {
 var entity = await GetByIdAsync(id);
 if (entity != null)
 {
 _dbSet.Remove(entity);
 await _context.SaveChangesAsync();
 }
 }
 }
}
