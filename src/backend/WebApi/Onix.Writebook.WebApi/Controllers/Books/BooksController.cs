using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Books.Application.Interfaces;
using Onix.Writebook.Books.Application.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Onix.Writebook.WebApi.Controllers.Books;

[Route("api/books")]
public class BooksController(
    INotificationContext notificationContext,
    IExceptionProcessor exceptionProcessor,
    IBookAppService bookPdfReportService)
    : BaseController(notificationContext, exceptionProcessor)
{
    private readonly IBookAppService _bookPdfReportService = bookPdfReportService;

    [HttpPost, Route("exportar/pdf")]
    public async Task<IActionResult> ExportBookPdf([FromBody] BookReportViewModel model, CancellationToken cancellationToken)
    {
        var pdfBytes = await _bookPdfReportService.ExportAsync(model, cancellationToken);

        var fileName = string.IsNullOrWhiteSpace(model.Title)
            ? "book.pdf"
            : $"{model.Title.Trim()}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
