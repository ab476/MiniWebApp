using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_permissions");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.Code)
               .HasColumnName("code")
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasColumnName("description");

        builder.Property(x => x.Category)
               .HasColumnName("category")
               .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
               .HasDatabaseName("ux_permissions_code")
               .IsUnique();
    }
}
