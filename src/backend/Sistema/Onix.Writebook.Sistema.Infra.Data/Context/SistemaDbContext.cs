using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Sistema.Infra.Data.Context
{
    public class SistemaDbContext : BaseDbContext
    {
        public SistemaDbContext(DbContextOptions<SistemaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaDbContext).Assembly);
        }
    }
}
