using AutoMapper;
using CoreArticle = CreaPrintCore.Models.Article;
using CoreCategory = CreaPrintCore.Models.Category;
using ApiArticle = CreaPrintApi.Dtos.Article;
using ApiCategory = CreaPrintApi.Dtos.Category;

namespace CreaPrintApi.Dtos
{
 public class MappingProfile : Profile
 {
 public MappingProfile()
 {
 CreateMap<CoreCategory, ApiCategory>().ReverseMap();
 CreateMap<CoreArticle, ApiArticle>().ReverseMap();
 }
 }
}
