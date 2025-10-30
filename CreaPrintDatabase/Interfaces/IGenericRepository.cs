using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintCore.Repositories
{
 public interface IGenericRepository<T> where T : BaseEntity
 {
 Task<IEnumerable<T>> GetAllAsync();
 Task<T?> GetByIdAsync(int id);
 Task<T> CreateAsync(T entity);
 Task UpdateAsync(T entity);
 Task DeleteAsync(int id);
 Task<int> GetCountAsync(Func<T, bool>? filter = null);
 Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
 }
}
