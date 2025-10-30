using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;

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
 article.CreatedOn = DateTime.UtcNow;
 return await _repository.CreateAsync(article);
 }

 public async Task<bool> UpdateAsync(int id, Article article)
 {
 if (id != article.Id) return false;

 // apply audit
 article.UpdatedOn = DateTime.UtcNow;

 // update directly without re-fetching the existing entity
 await _repository.UpdateAsync(article);
 return true;
 }

 public async Task DeleteAsync(int id)
 {
 await _repository.DeleteAsync(id);
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
