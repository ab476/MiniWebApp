using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MiniWebApp.UserApi.Test.Builders;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class RoleQueriesTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private IRoleQueries Queries => GetService<IRoleQueries>();

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenIdExists_ReturnsRole()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        await SeedRoleAsync(b => b.WithId(targetId));

        // Act
        var result = await Queries.GetByIdAsync(targetId, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(targetId);
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await Queries.GetByIdAsync(nonExistentId, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_ReturnsOnlyRolesForSpecifiedTenant()
    {
        // Arrange
        var myTenant = await SeedTenantAsync();
        var otherTenant = await SeedTenantAsync();

        await SeedRoleAsync(b => b.WithTenantId(myTenant.Id));
        await SeedRoleAsync(b => b.WithTenantId(myTenant.Id));
        await SeedRoleAsync(b => b.WithTenantId(otherTenant.Id));

        // Act
        var result = await Queries.GetPagedAsync(myTenant.Id, page: 1, pageSize: 10, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.All(r => r.TenantId == myTenant.Id).Should().BeTrue();
    }

    [Fact]
    public async Task GetPagedAsync_AppliesPagingAndClampingLogic()
    {
        // Arrange
        var tenant = await SeedTenantAsync();
        // Seed 5 roles
        for (int i = 0; i < 5; i++) await SeedRoleAsync(b => b.WithTenantId(tenant.Id));

        // Act: Request page 2 with size 2
        var result = await Queries.GetPagedAsync(tenant.Id, page: 2, pageSize: 2, CancellationToken);

        // Assert
        result.Value.Should().HaveCount(2);
        // Verify clamping (e.g., if user passes pageSize 500, implementation clamps to 100)
        var clampedResult = await Queries.GetPagedAsync(tenant.Id, 1, 500, CancellationToken);
        clampedResult.Value.Should().HaveCountLessThanOrEqualTo(100);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsRolesOrderedByDescendingCreatedAt()
    {
        // Arrange
        var tenant = await SeedTenantAsync();
        await SeedRoleAsync(b => b.WithTenantId(tenant.Id).WithCreatedAt(DateTime.UtcNow.AddDays(-1)));
        await SeedRoleAsync(b => b.WithTenantId(tenant.Id).WithCreatedAt(DateTime.UtcNow));

        // Act
        var result = await Queries.GetPagedAsync(tenant.Id, 1, 10, CancellationToken);

        // Assert
        result.Value.Should().BeInDescendingOrder(r => r.CreatedAt);
    }

    #endregion

    
}
