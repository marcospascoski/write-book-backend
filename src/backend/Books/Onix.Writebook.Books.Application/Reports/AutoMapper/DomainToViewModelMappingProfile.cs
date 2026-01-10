using AutoMapper;
using Onix.Writebook.Books.Application.Reports.ViewModels;
using Onix.Writebook.Books.Domain.Entities;

namespace Onix.Writebook.Books.Application.Reports.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<Book, BookReportViewModel>();
        CreateMap<Book, ExportBookPdfViewModel>();
    }
}
