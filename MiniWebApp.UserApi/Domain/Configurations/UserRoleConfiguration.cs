using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<TUserRole>
{
    public void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.ToTable("user_roles", "public");

        builder.HasKey(x => new { x.UserId, x.RoleId })
               .HasName("pk_user_roles");

        builder.Property(x => x.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.Property(x => x.RoleId)
               .HasColumnName("role_id")
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithMany(u => u.UserRoles)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_user_roles_user");

        builder.HasOne(x => x.Role)
               .WithMany(r => r.UserRoles)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_user_roles_role");
    }
}

