using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;
using Onix.Writebook.Acesso.Domain.Entities;

namespace Onix.Writebook.Acesso.Infra.Data.Context
{
    public class AcessosDbContext(DbContextOptions<AcessosDbContext> options) : BaseDbContext(options)
    {
        public DbSet<TokenRedefinicaoSenha> TokensRedefinicaoSenha { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureDefaultTypes(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcessosDbContext).Assembly);
        }
    }
}
