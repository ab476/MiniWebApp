namespace MiniWebApp.UserApi.Domain;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AppClaim> Claims => Set<AppClaim>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RoleClaim> RoleClaims => Set<RoleClaim>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        // Global soft delete filter
        builder.Entity<User>()
            .HasQueryFilter(u => u.Status == UserStatus.Active);

        base.OnModelCreating(builder);
    }
}
