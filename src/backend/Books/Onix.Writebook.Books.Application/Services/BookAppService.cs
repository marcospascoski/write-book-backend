using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Books.Application.Interfaces;
using Onix.Writebook.Books.Application.ViewModels;
using Onix.Writebook.Books.Domain.Entities;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Core.Resources;

namespace Onix.Writebook.Books.Application.Services;

public class BookAppService(
    IBookValidator bookValidator, 
    IMapper mapper
    ) : IBookAppService
{
    
    public async Task<byte[]> ExportAsync(BookReportViewModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var book = mapper.Map<Book>(model);

        if (await bookValidator.IsValid(book))
        {
            var html = BookHtmlTemplate.Render(model);
            return await PuppeteerPdfRenderer.RenderAsync(html, cancellationToken);
        }

        return [];
    }
}
