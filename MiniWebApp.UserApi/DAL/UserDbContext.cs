using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.DAL.Models;
using System.Data;
using System.Reflection.Emit;
using System.Security;

namespace MiniWebApp.UserApi.DAL;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        // Global soft delete filter
        builder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        base.OnModelCreating(builder);
    }
}
