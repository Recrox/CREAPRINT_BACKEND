using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;

namespace CreaPrintApi.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class ArticlesController : ControllerBase
 {
 private readonly IArticleService _service;

 public ArticlesController(IArticleService service)
 {
 _service = service;
 }

 [HttpGet]
 public async Task<ActionResult<IEnumerable<Article>>> GetAll()
 {
 var articles = await _service.GetAllAsync();
 return Ok(articles);
 }

 [HttpGet("{id}")]
 public async Task<ActionResult<Article>> GetById(int id)
 {
 var article = await _service.GetByIdAsync(id);
 if (article == null) return NotFound();
 return Ok(article);
 }

 [HttpPost]
 public async Task<ActionResult<Article>> Create(Article article)
 {
 var created = await _service.CreateAsync(article);
 return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
 }
 }
}
