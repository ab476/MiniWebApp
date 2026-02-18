using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<TRefreshToken>
{
    public void Configure(EntityTypeBuilder<TRefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_refresh_tokens");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.Property(x => x.TokenHash)
               .HasColumnName("token_hash")
               .HasMaxLength(512)
               .IsRequired();

        builder.Property(x => x.ExpiresAt)
               .HasColumnName("expires_at")
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.Property(x => x.RevokedAt)
               .HasColumnName("revoked_at");

        builder.Property(x => x.ReplacedByTokenId)
               .HasColumnName("replaced_by_token_id");

        builder.Property(x => x.CreatedByIp)
               .HasColumnName("created_by_ip")
               .HasMaxLength(100);

        // 🔹 FK → Users
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_refresh_tokens_user");

        // 🔹 Self-reference (token rotation chain)
        builder.HasOne(x => x.ReplacedByToken)
               .WithMany()
               .HasForeignKey(x => x.ReplacedByTokenId)
               .OnDelete(DeleteBehavior.SetNull)
               .HasConstraintName("fk_refresh_tokens_replaced_by");

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("ix_refresh_tokens_user_id");

        builder.HasIndex(x => x.ExpiresAt)
               .HasDatabaseName("ix_refresh_tokens_expires_at");
    }
}