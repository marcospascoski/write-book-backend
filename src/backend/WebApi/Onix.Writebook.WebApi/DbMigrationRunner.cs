using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Onix.Writebook.Acesso.Infra.Data.Context;
using Onix.Writebook.Sistema.Infra.Data.Context;
using Onix.Writebook.Books.Infra.Data.Context;

namespace Onix.Writebook.WebApi
{
    public static class DbMigrationRunner
    {
        public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
        {
            var applyMigrations = Environment.GetEnvironmentVariable("APPLY_MIGRATIONS");

            if (string.IsNullOrEmpty(applyMigrations) || !applyMigrations.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("APPLY_MIGRATIONS não está habilitado. Pulando migrations.");
                return;
            }

            Console.WriteLine("=========================================");
            Console.WriteLine("WriteBook API - Aplicando Migrations do Banco de Dados");
            Console.WriteLine("=========================================");

            try
            {
                using var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;

                // Aplicar migrations do Sistema
                Console.WriteLine("Aplicando migrations do Sistema...");
                var sistemaContext = services.GetRequiredService<SistemaDbContext>();
                await sistemaContext.Database.MigrateAsync();
                Console.WriteLine("✓ Migrations do Sistema aplicadas com sucesso!");

                // Aplicar migrations do Acesso
                Console.WriteLine("Aplicando migrations do Acesso...");
                var acessoContext = services.GetRequiredService<AcessosDbContext>();
                await acessoContext.Database.MigrateAsync();
                Console.WriteLine("✓ Migrations do Acesso aplicadas com sucesso!");

                // Aplicar migrations do Books
                Console.WriteLine("Aplicando migrations do Books...");
                var booksContext = services.GetRequiredService<BooksDbContext>();
                await booksContext.Database.MigrateAsync();
                Console.WriteLine("✓ Migrations do Books aplicadas com sucesso!");

                Console.WriteLine("=========================================");
                Console.WriteLine("Todas as migrations aplicadas com sucesso!");
                Console.WriteLine("=========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("❌ Erro ao aplicar migrations!");
                Console.WriteLine("=========================================");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("=========================================");

                // Não falhar a inicialização, apenas registrar o erro
                Console.WriteLine("⚠️  A aplicação continuará sem as migrations.");
            }
        }
    }
}
