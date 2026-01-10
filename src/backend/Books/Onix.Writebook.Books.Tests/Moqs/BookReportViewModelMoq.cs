using Onix.Framework.Tests;
using Onix.Writebook.Books.Application.ViewModels;

namespace Onix.Writebook.Books.Tests.Moqs;

public static class BookReportViewModelMoq
{
    public static BookReportViewModel CreateValid(
        Guid? id = null,
        Guid? usuarioId = null,
        string? title = null,
        string? content = null,
        int? wordCount = null,
        int? order = null,
        string? type = null)
    {
        var contentFinal = content ?? "Linha 1\n\nLinha 2";

        return new BookReportViewModel
        {
            Id = id ?? Guid.NewGuid(),
            UsuarioId = usuarioId ?? Guid.NewGuid(),
            Title = title ?? ValuesMoq.CreateString(20),
            Content = contentFinal,
            WordCount = wordCount ?? 4,
            Order = order ?? 1,
            Type = type ?? "Main"
        };
    }
}
