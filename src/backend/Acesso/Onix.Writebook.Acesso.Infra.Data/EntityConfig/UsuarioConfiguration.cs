using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onix.Framework.Security;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.ValueObjects;
using Onix.Writebook.Core.Domain.Enums;

namespace Onix.Writebook.Acesso.Infra.Data.EntityConfig
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);
            
            // Unique index mostly but handled by logic too.
            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.OwnsOne(x => x.Senha)
                .Property(x => x.Valor)
                .HasColumnName("Senha")
                .HasMaxLength(EncryptionHelper.CharactersCount(SenhaValueObject.EncryptionType))
                .IsRequired();

            builder.OwnsOne(x => x.Salt)
                .Property(x => x.Valor)
                .HasColumnName("Salt")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(Onix.Writebook.Acesso.Domain.Enums.EStatusUsuario.Inativo)
                .HasColumnType("smallint");

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.ToTable("Usuario");
        }
    }
}
