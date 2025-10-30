using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;
using System.Linq.Expressions;

namespace CreaPrintCore.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T?> UpdateAndGetAsync(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> DeleteAsync(int id);
        Task<int> GetCountAsync(Func<T, bool>? filter = null);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
    }
}
