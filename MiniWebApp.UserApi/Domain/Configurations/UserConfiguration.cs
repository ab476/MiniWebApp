using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "public");

        // Primary Key
        builder.HasKey(x => x.Id)
               .HasName("pk_users");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        // Tenant Link
        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        // Email & Normalization
        builder.Property(x => x.Email)
               .HasColumnName("email")
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.NormalizedEmail)
               .HasColumnName("normalized_email")
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.EmailConfirmed)
               .HasColumnName("email_confirmed")
               .HasDefaultValue(false)
               .IsRequired();

        // Username & Normalization
        builder.Property(x => x.UserName)
               .HasColumnName("username")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.NormalizedUsername)
               .HasColumnName("normalized_username")
               .HasMaxLength(100)
               .IsRequired();

        // Security & Status
        builder.Property(x => x.PasswordHash)
               .HasColumnName("password_hash")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasColumnName("status")
               .HasConversion<int>() // Ensures enum is stored as int in DB
               .IsRequired();

        builder.Property(x => x.FailedLoginAttempts)
               .HasColumnName("failed_login_attempts")
               .HasDefaultValue(0)
               .IsRequired();

        builder.Property(x => x.LockoutEnd)
               .HasColumnName("lockout_end");

        builder.Property(x => x.LastLoginAt)
               .HasColumnName("last_login_at");

        // Audit & Concurrency
        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasColumnName("created_by");

        builder.Property(x => x.UpdatedAt)
               .HasColumnName("updated_at");

        builder.Property(x => x.UpdatedBy)
               .HasColumnName("updated_by");

        // Relationships
        builder.HasOne(x => x.Tenant)
               .WithMany()
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_users_tenant");

        // Multi-tenant Unique Indexes
        builder.HasIndex(x => new { x.TenantId, x.NormalizedEmail })
               .HasDatabaseName("ux_users_tenant_normalized_email")
               .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.NormalizedUsername })
               .HasDatabaseName("ux_users_tenant_normalized_username")
               .IsUnique();

        // Performance Indexes
        builder.HasIndex(x => x.TenantId)
               .HasDatabaseName("ix_users_tenant_id");
    }
}