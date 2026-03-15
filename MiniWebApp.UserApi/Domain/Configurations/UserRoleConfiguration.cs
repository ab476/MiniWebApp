using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles", "public");

        // 1. Updated Composite Primary Key
        // Usually, UserRoles are uniquely identified by User + Role + Tenant
        builder.HasKey(x => new { x.UserId, x.RoleCode, x.TenantId })
               .HasName("pk_user_roles");

        builder.Property(x => x.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.Property(x => x.RoleCode)
               .HasColumnName("role_code") // Changed from role_id to match naming convention
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        // 2. Relationships
        builder.HasOne(x => x.User)
               .WithMany(u => u.UserRoles)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_user_roles_user");

        // FIX: Target both TenantId and RoleCode
        builder.HasOne(x => x.Role)
               .WithMany(r => r.UserRoles)
               .HasForeignKey(x => new { x.TenantId, x.RoleCode })
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_user_roles_role");
    }
}