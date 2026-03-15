using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.Core.Common;
using MiniWebApp.UserApi.Test.Builders.Roles;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class RoleRepositoryTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private IRoleRepository Repository => GetService<IRoleRepository>();

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_PersistsRoleAndReturns201()
    {
        // Arrange
        var tenant = await SeedTenantAsync();
        var request = CreateRoleRequestBuilder.Default.WithTenantId(tenant.Id).WithDisplayName("Manager");

        // Act
        var result = await Repository.CreateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value!.DisplayName.Should().Be("Manager");

        // Verify database state
        var dbRole = await GetFromDbAsync(result.Value.RoleCode, result.Value.TenantId);
        dbRole.Should().NotBeNull();
        dbRole!.DisplayName.Should().Be("Manager");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenRoleExists_UpdatesDatabaseAndReturns200()
    {
        // Arrange
        var role = await SeedRoleAsync(b => b.WithRandomRoleCode());
        var request = UpdateRoleRequestBuilder.Default.WithRoleCode(role.RoleCode).Build();

        // Act
        var result = await Repository.UpdateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        
        var updatedRole = await GetFromDbAsync(role.RoleCode, role.TenantId);
        updatedRole!.RoleCode.Should().Be("NewName");
        updatedRole.DisplayName.Should().Be("NEWNAME");
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleDoesNotExist_Returns404()
    {
        // Arrange
        var request = UpdateRoleRequestBuilder.Default.WithDisplayName("Name");

        // Act
        var result = await Repository.UpdateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenRoleExists_RemovesFromDatabase()
    {
        // Arrange
        var role = await SeedRoleAsync();

        // Act
        var result = await Repository.DeleteAsync(new DeleteRoleRequest(role.RoleCode, role.TenantId), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify it's actually gone
        var exists = await GetFromDbAsync(role.RoleCode, role.TenantId);
        exists.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleDoesNotExist_Returns404()
    {
        UpdateRoleRequest request = UpdateRoleRequestBuilder.Default.WithRoleCode("NonExistentRole").Build();
        // Act
        Outcome result = await Repository.DeleteAsync(new DeleteRoleRequest(request.RoleCode, Guid.NewGuid()), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    private async Task<Role?> GetFromDbAsync(string roleCode, Guid tenantId)
    {
        return await DbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.RoleCode == roleCode && t.TenantId == tenantId, CancellationToken);
    }
}