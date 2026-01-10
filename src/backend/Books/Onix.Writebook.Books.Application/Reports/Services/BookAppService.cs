using Onix.Writebook.Books.Application.Reports.Interfaces;
using Onix.Writebook.Books.Application.Reports.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Onix.Writebook.Books.Application.Reports.Services;

public class BookAppService : IBookAppService
{
    static BookAppService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Export(ExportBookPdfViewModel model)
    {
        var title = string.IsNullOrWhiteSpace(model.Title) ? "Livro" : model.Title.Trim();
        var author = string.IsNullOrWhiteSpace(model.Author) ? null : model.Author.Trim();
        var content = model.Content ?? string.Empty;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Column(col =>
                {
                    col.Item().Text(title).FontSize(20).SemiBold();
                    if (!string.IsNullOrWhiteSpace(author))
                        col.Item().Text(author).FontSize(11).FontColor(Colors.Grey.Darken2);
                });

                page.Content().PaddingTop(15).Text(text =>
                {
                    text.AlignLeft();

                    foreach (var line in content.Replace("\r\n", "\n").Split('\n'))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            text.Line(string.Empty);
                        else
                            text.Line(line);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.DefaultTextStyle(t => t.FontSize(10).FontColor(Colors.Grey.Darken1));
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
