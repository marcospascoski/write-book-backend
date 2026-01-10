using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Core.Domain.Enums;

namespace Onix.Writebook.Acesso.Infra.Data.EntityConfig
{
    public class PermissaoConfiguration : IEntityTypeConfiguration<Permissao>
    {
        public void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired(false);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.ToTable("Permissao");
        }
    }
}
