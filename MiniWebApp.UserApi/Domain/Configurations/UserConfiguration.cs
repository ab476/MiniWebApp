using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_users");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        builder.Property(x => x.Email)
               .HasColumnName("email")
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.NormalizedEmail)
               .HasColumnName("normalized_email")
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.Username)
               .HasColumnName("username")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.NormalizedUsername)
               .HasColumnName("normalized_username")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.PasswordHash)
               .HasColumnName("password_hash")
               .IsRequired();

        builder.Property(x => x.SecurityStamp)
               .HasColumnName("security_stamp")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Status)
               .HasColumnName("status")
               .IsRequired();

        builder.Property(x => x.FailedLoginAttempts)
               .HasColumnName("failed_login_attempts")
               .HasDefaultValue(0)
               .IsRequired();

        builder.Property(x => x.LockoutEnd)
               .HasColumnName("lockout_end");

        builder.Property(x => x.LastLoginAt)
               .HasColumnName("last_login_at");

        builder.Property(x => x.IsDeleted)
               .HasColumnName("is_deleted")
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasColumnName("created_by");

        builder.Property(x => x.UpdatedAt)
               .HasColumnName("updated_at");

        builder.Property(x => x.UpdatedBy)
               .HasColumnName("updated_by");

        builder.Property(x => x.RowVersion)
               .HasColumnName("row_version")
               .HasConversion(new DateTimeToTicksConverter())
               .IsConcurrencyToken(); 

        builder.HasOne(x => x.Tenant)
               .WithMany()
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_users_tenant");

        builder.HasIndex(x => new { x.TenantId, x.NormalizedEmail })
               .HasDatabaseName("ux_users_tenant_normalized_email")
               .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.NormalizedUsername })
               .HasDatabaseName("ux_users_tenant_normalized_username")
               .IsUnique();

        builder.HasIndex(x => x.TenantId)
               .HasDatabaseName("ix_users_tenant_id");

        builder.HasIndex(x => x.NormalizedEmail)
               .HasDatabaseName("ix_users_normalized_email");

        builder.HasIndex(x => x.IsDeleted)
               .HasDatabaseName("ix_users_is_deleted");
    }
}