using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Test.Builders.Tenants;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class TenantRepositoryTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private ITenantRepository Repository => GetService<ITenantRepository>();

    
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