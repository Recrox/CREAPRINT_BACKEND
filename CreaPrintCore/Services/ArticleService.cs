using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Interfaces;

namespace CreaPrintCore.Services
{
 public class ArticleService : IArticleService
 {
 private readonly IArticleRepository _repository;

 public ArticleService(IArticleRepository repository)
 {
 _repository = repository;
 }

 public async Task<IEnumerable<Article>> GetAllAsync()
 {
 return await _repository.GetAllAsync();
 }

 public async Task<Article?> GetByIdAsync(int id)
 {
 return await _repository.GetByIdAsync(id);
 }

 public async Task<Article> CreateAsync(Article article)
 {
 return await _repository.CreateAsync(article);
 }

 public async Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize)
 {
 return await _repository.GetPagedAsync(page, pageSize);
 }

 public async Task<int> GetCountAsync(Func<Article, bool>? filter = null)
 {
 return await _repository.GetCountAsync(filter);
 }
 }
}
