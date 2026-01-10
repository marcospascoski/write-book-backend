using System.Net;
using System.Net.Http.Json;
using System.Text;
using Onix.Writebook.Books.Application.Reports.ViewModels;
using Onix.Writebook.Books.Tests.Infrastructure;
using Xunit;

namespace Onix.Writebook.Books.Tests.Reports;

public class ExportBookPdfEndpointTests : IClassFixture<WritebookWebApiFactory>
{
    private readonly HttpClient _client;

    public ExportBookPdfEndpointTests(WritebookWebApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ExportBookPdf_ReturnsPdfBytes()
    {
        var model = new ExportBookPdfViewModel
        {
            Title = "Meu Livro",
            Author = "Autor",
            Content = "Linha 1\n\nLinha 2"
        };

        using var response = await _client.PostAsJsonAsync("/api/books/reports/book/pdf", model);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);

        var header = Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 5));
        Assert.Equal("%PDF-", header);

        if (response.Content.Headers.ContentType is not null)
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.MediaType);
    }
}
