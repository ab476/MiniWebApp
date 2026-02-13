using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiniWebApp.TaskAPI.Domain.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // snake_case table
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .HasColumnName("priority")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.DueDate)
            .HasColumnName("due_date");

        // ---- Concurrency token using ticks ----
        var ticksConverter = new DateTimeToTicksConverter();

        builder.Property(t => t.LastModified)
            .HasColumnName("last_modified_ticks")
            .HasConversion(ticksConverter)
            .IsConcurrencyToken()
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.Status)
            .HasDatabaseName("ix_tasks_status");

        builder.HasIndex(t => t.DueDate)
            .HasDatabaseName("ix_tasks_due_date");

        builder.HasIndex(t => t.LastModified)
           .HasDatabaseName("ix_tasks_last_modified_ticks");
    }
}
