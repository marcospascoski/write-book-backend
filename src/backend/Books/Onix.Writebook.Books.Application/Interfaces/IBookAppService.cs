using Onix.Writebook.Books.Application.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Application.Interfaces;

public interface IBookAppService
{
    Task<byte[]> ExportAsync(BookReportViewModel model, CancellationToken cancellationToken = default);
}
