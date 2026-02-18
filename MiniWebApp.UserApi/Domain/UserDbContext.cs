using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Domain.Models;

namespace MiniWebApp.UserApi.Domain;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<TTenant> TTenants => Set<TTenant>();
    public DbSet<TUser> TUsers => Set<TUser>();
    public DbSet<TRole> TRoles => Set<TRole>();
    public DbSet<TPermission> TPermissions => Set<TPermission>();
    public DbSet<TUserRole> TUserRoles => Set<TUserRole>();
    public DbSet<TRolePermission> TRolePermissions => Set<TRolePermission>();
    public DbSet<TRefreshToken> TRefreshTokens => Set<TRefreshToken>();
    public DbSet<TLoginHistory> TLoginHistories => Set<TLoginHistory>();
    public DbSet<TAuditLog> TAuditLogs => Set<TAuditLog>();
    public DbSet<TOutboxMessage> TOutboxMessages => Set<TOutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        // Global soft delete filter
        builder.Entity<TUser>()
            .HasQueryFilter(u => !u.IsDeleted);

        base.OnModelCreating(builder);
    }
}
