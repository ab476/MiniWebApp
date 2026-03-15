using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "public");

        // 1. Composite Primary Key: RoleCode + TenantId
        builder.HasKey(x => new { x.TenantId, x.RoleCode })
               .HasName("pk_roles");

        builder.Property(x => x.RoleCode)
               .HasColumnName("role_code") // Updated from 'name' to match property
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        // 2. Remaining Properties
        builder.Property(x => x.DisplayName)
               .HasColumnName("display_name") // Updated from 'description'
               .HasMaxLength(250);

        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Database-side default

        builder.Property(x => x.CreatedBy)
               .HasColumnName("created_by");

        // 3. Relationships
        builder.HasOne(x => x.Tenant)
               .WithMany() // Assuming Tenant doesn't have a collection of Roles
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_roles_tenant");

        // 4. Indexes
        // PK already covers (TenantId, RoleCode), but an index on TenantId alone
        // helps with filtering by tenant.
        builder.HasIndex(x => x.TenantId)
               .HasDatabaseName("ix_roles_tenant_id");
    }
}
