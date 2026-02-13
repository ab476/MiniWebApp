using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.DAL.Models;

namespace MiniWebApp.UserApi.DAL.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions", "public");

        builder.HasKey(x => new { x.RoleId, x.PermissionId })
               .HasName("pk_role_permissions");

        builder.Property(x => x.RoleId)
               .HasColumnName("role_id")
               .IsRequired();

        builder.Property(x => x.PermissionId)
               .HasColumnName("permission_id")
               .IsRequired();

        builder.HasOne(x => x.Role)
               .WithMany(r => r.RolePermissions)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_role_permissions_role");

        builder.HasOne(x => x.Permission)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(x => x.PermissionId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_role_permissions_permission");
    }
}

