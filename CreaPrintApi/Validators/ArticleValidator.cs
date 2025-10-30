using FluentValidation;
using CreaPrintCore.Models;

namespace CreaPrintApi.Validators;

public class ArticleValidator : AbstractValidator<Article>
{
 public ArticleValidator()
 {
 RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
 RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
 RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
 RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
 }
}
