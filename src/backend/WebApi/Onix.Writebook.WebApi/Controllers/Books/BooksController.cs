using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Books.Application.Reports.Interfaces;
using Onix.Writebook.Books.Application.Reports.ViewModels;
using System;

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
    public IActionResult ExportBookPdf(ExportBookPdfViewModel model)
    {
        var pdfBytes = _bookPdfReportService.Export(model);

        var fileName = string.IsNullOrWhiteSpace(model.Title)
            ? "book.pdf"
            : $"{model.Title.Trim()}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
