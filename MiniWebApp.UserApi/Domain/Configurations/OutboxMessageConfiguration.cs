using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "public");

        builder.HasKey(x => x.Id)
               .HasName("pk_outbox_messages");

        builder.Property(x => x.Id)
               .HasColumnName("id");

        builder.Property(x => x.AggregateType)
               .HasColumnName("aggregate_type")
               .HasMaxLength(200);

        builder.Property(x => x.AggregateId)
               .HasColumnName("aggregate_id");

        builder.Property(x => x.Type)
               .HasColumnName("type")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Payload)
               .HasColumnName("payload")
               .IsRequired();

        builder.Property(x => x.OccurredOn)
               .HasColumnName("occurred_on")
               .IsRequired();

        builder.Property(x => x.ProcessedOn)
               .HasColumnName("processed_on");

        builder.Property(x => x.Error)
               .HasColumnName("error");

        builder.HasIndex(x => x.ProcessedOn)
               .HasDatabaseName("ix_outbox_messages_processed_on");
    }
}

