using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintCore.Repositories
{
 public interface IGenericRepository<T> where T : BaseItem
 {
 Task<IEnumerable<T>> GetAllAsync();
 Task<T?> GetByIdAsync(int id);
 Task<T> CreateAsync(T entity);
 Task UpdateAsync(T entity);
 Task DeleteAsync(int id);
 }
}
