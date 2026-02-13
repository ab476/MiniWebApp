using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.DAL.Models;

namespace MiniWebApp.UserApi.DAL.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_audit_logs");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.TenantId)
               .HasColumnName("tenant_id")
               .IsRequired();

        builder.Property(x => x.EntityName)
               .HasColumnName("entity_name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.EntityId)
               .HasColumnName("entity_id")
               .IsRequired();

        builder.Property(x => x.Action)
               .HasColumnName("action")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.OldValues)
               .HasColumnName("old_values");

        builder.Property(x => x.NewValues)
               .HasColumnName("new_values");

        builder.Property(x => x.PerformedBy)
               .HasColumnName("performed_by");

        builder.Property(x => x.PerformedAt)
               .HasColumnName("performed_at")
               .IsRequired();

        builder.Property(x => x.CorrelationId)
               .HasColumnName("correlation_id")
               .HasMaxLength(100);

        builder.HasOne(x => x.Tenant)
               .WithMany()
               .HasForeignKey(x => x.TenantId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_audit_logs_tenant");

        builder.HasIndex(x => x.TenantId)
               .HasDatabaseName("ix_audit_logs_tenant_id");

        builder.HasIndex(x => new { x.EntityName, x.EntityId })
               .HasDatabaseName("ix_audit_logs_entity");
    }
}
