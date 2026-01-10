using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onix.Writebook.Acesso.Domain.Entities;

namespace Onix.Writebook.Acesso.Infra.Data.EntityConfig
{
    public class PerfilPermissaoConfiguration : IEntityTypeConfiguration<PerfilPermissao>
    {
        public void Configure(EntityTypeBuilder<PerfilPermissao> builder)
        {
            builder.HasKey(pp => pp.Id);

            builder.Property(pp => pp.PerfilId)
                .IsRequired();

            builder.Property(pp => pp.PermissaoId)
                .IsRequired();

            builder.HasOne(pp => pp.Perfil)
                .WithMany(p => p.PerfilPermissoes)
                .HasForeignKey(pp => pp.PerfilId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pp => pp.Permissao)
                .WithMany()
                .HasForeignKey(pp => pp.PermissaoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("PerfilPermissao");
        }
    }
}
