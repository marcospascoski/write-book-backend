using AutoMapper;
using Onix.Writebook.Books.Application.ViewModels;
using Onix.Writebook.Books.Domain.Entities;

namespace Onix.Writebook.Books.Application.AutoMapper;

public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        CreateMap<BookReportViewModel, Book>();
    }
}
