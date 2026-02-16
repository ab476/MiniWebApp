using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("login_history", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_login_history");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.UserId)
               .HasColumnName("user_id")
               .IsRequired();

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        builder.Property(x => x.LoginTime)
               .HasColumnName("login_time")
               .IsRequired();

        builder.Property(x => x.IpAddress)
               .HasColumnName("ip_address")
               .HasMaxLength(100);

        builder.Property(x => x.DeviceInfo)
               .HasColumnName("device_info");

        builder.Property(x => x.Location)
               .HasColumnName("location")
               .HasMaxLength(200);

        builder.Property(x => x.IsSuccessful)
               .HasColumnName("is_successful")
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_login_history_user");

        builder.HasOne(x => x.Tenant)
               .WithMany()
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_login_history_tenant");

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("ix_login_history_user_id");

        builder.HasIndex(x => x.LoginTime)
               .HasDatabaseName("ix_login_history_login_time");
    }
}

