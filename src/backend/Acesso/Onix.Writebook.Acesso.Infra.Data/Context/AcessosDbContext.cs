using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Acesso.Infra.Data.Context
{
    public class AcessosDbContext : BaseDbContext
    {
        public AcessosDbContext(DbContextOptions<AcessosDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureDefaultTypes(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcessosDbContext).Assembly);
        }
    }
}
