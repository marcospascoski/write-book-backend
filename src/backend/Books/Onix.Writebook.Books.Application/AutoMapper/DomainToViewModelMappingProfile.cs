using AutoMapper;
using Onix.Writebook.Books.Application.ViewModels;
using Onix.Writebook.Books.Domain.Entities;

namespace Onix.Writebook.Books.Application.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<Book, BookReportViewModel>();
    }
}
