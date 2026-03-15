using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppClaim = MiniWebApp.UserApi.Domain.Models.AppClaim;

namespace MiniWebApp.UserApi.Domain.Configurations;

public class AppClaimConfiguration : IEntityTypeConfiguration<AppClaim>
{
    public void Configure(EntityTypeBuilder<AppClaim> builder)
    {
        // Renamed from 'permissions' to 'claims'
        builder.ToTable("claims", "public");

        // 1. Primary Key (Natural Key)
        builder.HasKey(x => x.ClaimCode)
               .HasName("pk_claims");

        builder.Property(x => x.ClaimCode)
               .HasColumnName("claim_code")
               .HasMaxLength(50)
               .IsRequired()
               .HasConversion(
                   v => v.ToLowerInvariant().Trim(),
                   v => v
               );

        // 2. Metadata
        builder.Property(x => x.Description)
               .HasColumnName("description")
               .HasMaxLength(500);

        builder.Property(x => x.Category)
               .HasColumnName("category")
               .HasMaxLength(50)
               .IsRequired();

        // 3. Relationships
        builder.HasMany(x => x.RoleClaims) // Assuming RolePermission was renamed to RoleClaim
               .WithOne(x => x.Claim)
               .HasForeignKey(x => x.ClaimCode)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_role_claims_claim");
    }
}
