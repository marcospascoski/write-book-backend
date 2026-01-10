using Onix.Framework.Tests;
using Onix.Writebook.Books.Domain.Entities;

namespace Onix.Writebook.Books.Tests.Moqs;

public static class BookMoq
{
    private const string NotProvided = "__NOT_PROVIDED__";

    public static Book Create(
        Guid? id = null,
        Guid? usuarioId = null,
        string? title = NotProvided,
        string? content = NotProvided,
        int? wordCount = null,
        int? order = null)
    {
        var titleFinal = title == NotProvided ? ValuesMoq.CreateString(20) : title;
        var contentFinal = content == NotProvided ? "Linha 1\n\nLinha 2" : content;

        return new Book(
            id ?? Guid.NewGuid(),
            usuarioId ?? Guid.NewGuid(),
            titleFinal!,
            contentFinal!,
            wordCount ?? 4,
            order ?? 1
        );
    }
}
