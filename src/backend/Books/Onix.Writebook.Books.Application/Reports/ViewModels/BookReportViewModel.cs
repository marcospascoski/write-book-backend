namespace Onix.Writebook.Books.Application.Reports.ViewModels;

public class BookReportViewModel
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int WordCount { get; set; }
    public int Order { get; set; }
    public string? Type { get; set; }
}
