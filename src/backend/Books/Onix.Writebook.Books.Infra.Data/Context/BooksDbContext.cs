using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Books.Infra.Data.Context
{
    public class BooksDbContext(DbContextOptions<BooksDbContext> options) : BaseDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureDefaultTypes(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BooksDbContext).Assembly);
        }
    }
}
