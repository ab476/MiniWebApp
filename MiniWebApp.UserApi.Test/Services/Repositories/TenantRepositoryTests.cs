using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Test.Builders.Tenants;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class TenantRepositoryTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private ITenantRepository Repository => GetService<ITenantRepository>();

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenTenantExists_ReturnsSuccess()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var tenant = await SeedTenantAsync(b => b.WithId(targetId).WithName("Test Corp"));

        // Act
        var result = await Repository.GetByIdAsync(targetId, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(targetId);
        result.Value.Name.Should().Be("Test Corp");
    }

    [Fact]
    public async Task GetByIdAsync_WhenTenantDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result = await Repository.GetByIdAsync(Guid.NewGuid(), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Error.Should().Be("Tenant not found.");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMultipleTenantsExist_ReturnsCorrectTenant()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var tenants = await SeedTenantsAsync(
             b => b.WithName("Other Corp"), 
             b => b.WithId(targetId).WithName("Target Corp")
        );
        

        // Act
        var result = await Repository.GetByIdAsync(targetId, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Target Corp");
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_ShouldReturnTenantsOrderedByNewestFirst()
    {
        // Arrange
        var tenants = await SeedTenantsAsync(
            b => b.WithName("Oldest").WithCreatedAt(DateTime.UtcNow.AddDays(-1)), 
            b => b.WithName("Newest").WithCreatedAt(DateTime.UtcNow)
        ); 

        // Act
        var result = await Repository.GetPagedAsync(page: 1, pageSize: 1, CancellationToken);

        // Assert
        result.Value.Should().HaveCount(1);
        result.Value!.First().Name.Should().Be("Newest");
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageIsZero_ShouldDefaultToPageOne()
    {
        // Arrange
        await SeedTenantAsync();

        // Act
        var result = await Repository.GetPagedAsync(page: 0, pageSize: 10, CancellationToken);

        // Assert
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageSizeIsTooLarge_ShouldClampToLimit()
    {
        // Arrange 
        var manyTenants = await SeedTenantsAsync(110);

        // Act
        var result = await Repository.GetPagedAsync(page: 1, pageSize: 500, CancellationToken);

        // Assert
        // Assuming your repository has a MaxPageSize of 100
        result.Value.Should().HaveCount(100);
    }

    #endregion

    #region Write Operation Tests

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ReturnsCreatedTenant()
    {
        // Arrange
        var request = CreateTenantRequestBuilder.Default.Build();

        // Act
        var result = await Repository.CreateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify Persistence
        var dbTenant = await GetFromDbAsync(result.Value!.Id);
        dbTenant.Should().NotBeNull();
        dbTenant!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenTenantExists_UpdatesFields()
    {
        // Arrange
        var tenant = await SeedTenantAsync(b => b.WithName("Old Name"));
        var request = UpdateTenantRequestBuilder.Default.WithName("New Shiny Name").Build();

        // Act
        var result = await Repository.UpdateAsync(tenant.Id, request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify Change
        var updated = await GetFromDbAsync(tenant.Id);
        updated!.Name.Should().Be("New Shiny Name");
    }

    [Fact]
    public async Task DeleteAsync_WhenTenantExists_RemovesFromDatabase()
    {
        // Arrange
        var tenant = await SeedTenantAsync();

        // Act
        var result = await Repository.DeleteAsync(tenant.Id, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exists = await DbContext.Tenants.AnyAsync(t => t.Id == tenant.Id, CancellationToken);
        exists.Should().BeFalse();
    }

    #endregion

    #region Status Operation Tests

    [Fact]
    public async Task DeactivateAsync_WhenTenantIsActive_SetsIsActiveToFalse()
    {
        // Arrange
        var tenant = await SeedTenantAsync(b => b.WithIsActive(true));
        var request = DeactivateTenantRequestBuilder.Default.WithTenantId(tenant.Id).Build();

        // Act
        var result = await Repository.DeactivateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updated = await GetFromDbAsync(tenant.Id);
        updated!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ActivateAsync_WhenTenantIsInactive_SetsIsActiveToTrue()
    {
        // Arrange
        var tenant = await SeedTenantAsync(b => b.WithIsActive(false));
        var request = ActivateTenantRequestBuilder.Default.WithTenantId(tenant.Id).Build();

        // Act
        var result = await Repository.ActivateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updated = await GetFromDbAsync(tenant.Id);
        updated!.IsActive.Should().BeTrue();
    }

    #endregion

    #region Private Helpers
    

    private async Task<List<Tenant>> SeedTenantsAsync(int count, Action<TenantBuilder, int>? configure = null)
    {
        var actions = Enumerable.Range(0, count)
            .Select(i => (Action<TenantBuilder>)(builder => configure?.Invoke(builder, i)))
            .ToArray();

        return await SeedTenantsAsync(actions);
    }

    private async Task<Tenant?> GetFromDbAsync(Guid id)
    {
        return await DbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, CancellationToken);
    }

    #endregion
}