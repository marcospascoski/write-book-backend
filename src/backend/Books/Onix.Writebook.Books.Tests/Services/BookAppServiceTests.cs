using AutoMapper;
using Onix.Writebook.Books.Application.Services;
using Onix.Writebook.Books.Domain.Entities;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Books.Tests.Moqs;
using Onix.Writebook.Books.Infra.Data.Context;
using Xunit;
using System.Text;

namespace Onix.Writebook.Books.Tests.Services;

public class BookAppServiceTests(IBookValidator bookValidator, IMapper mapper, BooksDbContext dbContext)
{
    private readonly BookAppService _bookAppService = new(bookValidator, mapper);
    private readonly BooksDbContext _dbContext = dbContext;

    private static readonly char[] WordSplitSeparators = [' ', '\n', '\r'];

    [Fact]
    public async Task ExportAsync_Deve_retornar_bytes_quando_book_valido()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        var model = BookReportViewModelMoq.CreateValid(usuarioId: usuarioId);

        // Act
        var result = await _bookAppService.ExportAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ExportAsync_Deve_retornar_array_vazio_quando_book_invalido()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        var model = BookReportViewModelMoq.CreateValid(usuarioId: usuarioId, title: "");

        // Act
        var result = await _bookAppService.ExportAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_Deve_exportar_livro_com_muito_conteudo()
    {
        // Arrange: Create a usuario in the database first
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario(usuarioId);
        await _dbContext.Set<Usuario>().AddAsync(usuario);
        await _dbContext.SaveChangesAsync();

        // Criar um conteúdo extenso simulando um livro real com múltiplos capítulos e parágrafos
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("# Capítulo 1: A Jornada Começa");
        contentBuilder.AppendLine();
        contentBuilder.AppendLine("Era uma vez, em uma terra distante, um desenvolvedor que sonhava em criar o melhor sistema de escrita de livros do mundo.");
        contentBuilder.AppendLine("Ele trabalhava dia e noite, escrevendo código e testes, sempre buscando a perfeição.");
        contentBuilder.AppendLine();

        for (int capitulo = 2; capitulo <= 20; capitulo++)
        {
            contentBuilder.AppendLine($"# Capítulo {capitulo}: Continuando a Aventura");
            contentBuilder.AppendLine();

            for (int paragrafo = 1; paragrafo <= 10; paragrafo++)
            {
                contentBuilder.AppendLine($"Este é o parágrafo {paragrafo} do capítulo {capitulo}. " +
                    "O sistema precisa ser capaz de lidar com grandes volumes de texto, " +
                    "pois autores frequentemente escrevem livros extensos com milhares de palavras. " +
                    "Cada parágrafo pode conter várias frases, e cada frase pode ter ideias complexas " +
                    "que precisam ser preservadas durante a exportação para PDF.");
                contentBuilder.AppendLine();
            }
        }

        contentBuilder.AppendLine("# Epílogo: O Sistema Funcionou!");
        contentBuilder.AppendLine();
        contentBuilder.AppendLine("E assim, o sistema de escrita de livros provou ser robusto o suficiente para lidar com livros completos.");

        var longContent = contentBuilder.ToString();
        var wordCount = longContent.Split(WordSplitSeparators, StringSplitOptions.RemoveEmptyEntries).Length;

        var model = BookReportViewModelMoq.CreateValid(
            usuarioId: usuarioId,
            content: longContent,
            wordCount: wordCount,
            title: "O Grande Livro de Testes"
        );

        // Act
        var result = await _bookAppService.ExportAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        // Verifica que o PDF gerado tem um tamanho razoável (maior que 1KB)
        Assert.True(result.Length > 1024, $"O PDF deveria ter mais de 1KB, mas tem apenas {result.Length} bytes");
    }
}
