using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniWebApp.TaskAPI.Domain.Configurations;

namespace MiniWebApp.TaskAPI.Domain;

public class TaskAPIDbContext(DbContextOptions<TaskAPIDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<NoteTag> NoteTags => Set<NoteTag>();
    public DbSet<Product> Products => Set<Product>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>()
            .HaveConversion<GuidToBytesConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskItemConfiguration).Assembly);

        // Note → Tags (1-to-many)
        modelBuilder.Entity<NoteTag>()
            .HasIndex(t => new { t.NoteId, t.Tag })
            .IsUnique();

        // Soft delete filter
        modelBuilder.Entity<Note>()
            .HasQueryFilter(n => !n.IsDeleted);
    }
}
