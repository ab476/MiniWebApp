using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class TenantHistoryConfiguration : IEntityTypeConfiguration<TenantHistory>
{
    public void Configure(EntityTypeBuilder<TenantHistory> builder)
    {
        // Table naming following your "tenants" pattern
        builder.ToTable("tenant_histories");

        // Primary Key for the history record itself
        builder.HasKey(x => x.HistoryId)
               .HasName("pk_tenant_histories");

        builder.Property(x => x.HistoryId)
               .HasColumnName("history_id");

        // Reference to the original Tenant ID
        builder.Property(x => x.EntityId)
               .HasColumnName("entity_id")
               .IsRequired();

        builder.Property(x => x.Name)
               .HasColumnName("name")
               .HasMaxLength(200);

        builder.Property(x => x.Domain)
               .HasColumnName("domain")
               .HasMaxLength(200);

        builder.Property(x => x.IsActive)
               .HasColumnName("is_active");

        // Audit specific fields
        builder.Property(x => x.Action)
               .HasColumnName("action")
               .HasConversion<string>() // Stores 'Insert', 'Update', 'Delete' as strings
               .IsRequired();

        builder.Property(x => x.ChangedBy)
               .HasColumnName("changed_by");

        builder.Property(x => x.ChangedAt)
               .HasColumnName("changed_at")
               .IsRequired();

        // Indexing EntityId for faster history lookups per tenant
        builder.HasIndex(x => x.EntityId)
               .HasDatabaseName("ix_tenant_histories_entity_id");
    }
}