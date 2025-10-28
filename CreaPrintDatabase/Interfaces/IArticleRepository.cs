using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintDatabase.Interfaces
{
 public interface IArticleRepository
 {
 Task<IEnumerable<Article>> GetAllAsync();
 Task<Article?> GetByIdAsync(int id);
 Task<Article> CreateAsync(Article article);
 }
}
