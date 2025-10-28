using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintDatabase.Interfaces;

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
 }
}
