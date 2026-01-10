using Onix.Writebook.Books.Application.Reports.ViewModels;

namespace Onix.Writebook.Books.Application.Reports.Interfaces;

public interface IBookAppService
{
    byte[] Export(ExportBookPdfViewModel model);
}
