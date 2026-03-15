using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("role_claims", "public");

        // 1. Composite Primary Key (TenantId + RoleCode + ClaimCode)
        // This ensures a specific role in a specific tenant can't have the same claim twice.
        builder.HasKey(x => new { x.TenantId, x.RoleCode, x.ClaimCode })
               .HasName("pk_role_claims");

        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.RoleCode).HasColumnName("role_code").HasMaxLength(50);
        builder.Property(x => x.ClaimCode).HasColumnName("claim_code").HasMaxLength(50);

        // 2. Relationship to Role (Uses Composite Foreign Key)
        builder.HasOne(x => x.Role)
               .WithMany(r => r.RoleClaims)
               .HasForeignKey(x => new { x.TenantId, x.RoleCode })
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_role_claims_role");

        // 3. Relationship to AppClaim (Uses Natural Foreign Key)
        builder.HasOne(x => x.Claim)
               .WithMany(c => c.RoleClaims)
               .HasForeignKey(x => x.ClaimCode)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_role_claims_claim");
    }
}