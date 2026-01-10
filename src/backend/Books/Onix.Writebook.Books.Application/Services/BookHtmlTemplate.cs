using System.Net;
using System.Text;
using Onix.Writebook.Books.Application.ViewModels;

namespace Onix.Writebook.Books.Application.Services;

internal static class BookHtmlTemplate
{
    public static string Render(BookReportViewModel model)
    {
        var title = string.IsNullOrWhiteSpace(model.Title) ? "Livro" : model.Title.Trim();
        var content = model.Content ?? string.Empty;

        static string HtmlEncode(string value) => WebUtility.HtmlEncode(value);

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"pt-br\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        sb.AppendLine($"  <title>{HtmlEncode(title)}</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    @page { size: A4; margin: 40px; }");
        sb.AppendLine("    body { font-family: Arial, Helvetica, sans-serif; font-size: 12pt; color: #111; }");
        sb.AppendLine("    header { margin-bottom: 16px; }");
        sb.AppendLine("    h1 { font-size: 20pt; margin: 0; }");
        sb.AppendLine("    .meta { margin-top: 6px; color: #555; font-size: 10pt; }");
        sb.AppendLine("    .content { white-space: pre-wrap; line-height: 1.35; }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <header>");
        sb.AppendLine($"    <h1>{HtmlEncode(title)}</h1>");
        sb.AppendLine($"    <div class=\"meta\">Palavras: {model.WordCount} | Ordem: {model.Order}{(string.IsNullOrWhiteSpace(model.Type) ? string.Empty : $" | Tipo: {HtmlEncode(model.Type)}")}</div>");
        sb.AppendLine("  </header>");
        sb.AppendLine($"  <main class=\"content\">{HtmlEncode(content)}</main>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}
