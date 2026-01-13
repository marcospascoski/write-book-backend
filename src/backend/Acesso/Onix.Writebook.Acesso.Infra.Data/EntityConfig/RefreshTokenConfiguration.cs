using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onix.Writebook.Acesso.Domain.Entities;

namespace Onix.Writebook.Acesso.Infra.Data.EntityConfig
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Token)
                .IsRequired()
                .HasMaxLength(64); // Base64 de 32 bytes = 44 caracteres, com margem de segurança

            builder.HasIndex(c => c.Token)
                .IsUnique();

            builder.Property(c => c.UsuarioId)
                .IsRequired();

            builder.Property(c => c.DataExpiracao)
                .IsRequired();

            builder.Property(c => c.Revogado)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IPAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(c => c.UserAgent)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("RefreshToken");
        }
    }
}
