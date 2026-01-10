using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Books.Tests.Moqs;
using Onix.Writebook.Books.Infra.Data.Context;
using Onix.Writebook.Books.Domain.Entities;
using Xunit;

namespace Onix.Writebook.Books.Tests.Validators;

public class BookValidatorTests(
    IBookValidator bookValidator,
    INotificationContext notificationContext,
    BooksDbContext dbContext)
{
    private readonly IBookValidator _bookValidator = bookValidator;
    private readonly INotificationContext _notificationContext = notificationContext;
    private readonly BooksDbContext _dbContext = dbContext;

    [Fact]
    public async Task Deve_validar_book_valido()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        var book = BookMoq.Create(usuarioId: usuarioId);

        // Act
        var result = await _bookValidator.IsValid(book);

        // Assert
        Assert.True(result);
        Assert.False(_notificationContext.HasErrors);
    }

    [Fact]
    public async Task Deve_invalidar_book_com_id_vazio()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        var book = BookMoq.Create(id: Guid.Empty, usuarioId: usuarioId);

        // Act
        var result = await _bookValidator.IsValid(book);

        // Assert
        Assert.False(result);
        Assert.True(_notificationContext.HasErrors);
    }

    [Fact]
    public async Task Deve_invalidar_book_com_title_vazio()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        var book = BookMoq.Create(title: "", usuarioId: usuarioId);

        // Act
        var result = await _bookValidator.IsValid(book);

        // Assert
        Assert.False(result);
        Assert.True(_notificationContext.HasErrors);
    }
}
