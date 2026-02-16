using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(x => x.Id)
               .HasName("pk_tenants");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.Name)
               .HasColumnName("name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Domain)
               .HasColumnName("domain")
               .HasMaxLength(200);

        builder.Property(x => x.IsActive)
               .HasColumnName("is_active")
               .HasDefaultValue(true)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .HasColumnName("updated_at");

        builder.HasIndex(x => x.Domain)
               .HasDatabaseName("ux_tenants_domain")
               .IsUnique();
    }
}