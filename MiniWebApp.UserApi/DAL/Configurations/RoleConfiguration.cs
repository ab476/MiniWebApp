using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.DAL.Models;

namespace MiniWebApp.UserApi.DAL.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_roles");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        builder.Property(x => x.Name)
               .HasColumnName("name")
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.NormalizedName)
               .HasColumnName("normalized_name")
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasColumnName("description");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.HasOne(x => x.Tenant)
               .WithMany()
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_roles_tenant");

        builder.HasIndex(x => new { x.TenantId, x.NormalizedName })
               .HasDatabaseName("ux_roles_tenant_normalized_name")
               .IsUnique();

        builder.HasIndex(x => x.TenantId)
               .HasDatabaseName("ix_roles_tenant_id");
    }
}
