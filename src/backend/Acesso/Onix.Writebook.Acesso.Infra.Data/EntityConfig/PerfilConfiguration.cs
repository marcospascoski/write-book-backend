using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Core.Domain.Enums;

namespace Onix.Writebook.Acesso.Infra.Data.EntityConfig
{
    public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired(false);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.HasMany(p => p.PerfilPermissoes)
                .WithOne(pp => pp.Perfil)
                .HasForeignKey(pp => pp.PerfilId);

            builder.ToTable("Perfil");
        }
    }
}
