using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Models.Roles;
using MiniWebApp.UserApi.Test.Builders;

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
        var request = CreateRoleRequestBuilder.Default.WithTenantId(tenant.Id).WithName("Manager").WithDescription("Manages the team");

        // Act
        var result = await Repository.CreateAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value!.Name.Should().Be("Manager");

        // Verify database state
        var dbRole = await GetFromDbAsync(result.Value.Id);
        dbRole.Should().NotBeNull();
        dbRole!.NormalizedName.Should().Be("MANAGER");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenRoleExists_UpdatesDatabaseAndReturns200()
    {
        // Arrange
        var role = await SeedRoleAsync(b => b.WithName("OldName"));
        var request = UpdateRoleRequestBuilder.Default.WithName("NewName").WithDescription("Updated Description");

        // Act
        var result = await Repository.UpdateAsync(role.Id, request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        
        var updatedRole = await GetFromDbAsync(role.Id);
        updatedRole!.Name.Should().Be("NewName");
        updatedRole.NormalizedName.Should().Be("NEWNAME");
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleDoesNotExist_Returns404()
    {
        // Arrange
        var request = UpdateRoleRequestBuilder.Default.WithName("Name").WithDescription("Desc");

        // Act
        var result = await Repository.UpdateAsync(Guid.NewGuid(), request, CancellationToken);

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
        var result = await Repository.DeleteAsync(role.Id, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify it's actually gone
        var exists = await GetFromDbAsync(role.Id);
        exists.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleDoesNotExist_Returns404()
    {
        // Act
        var result = await Repository.DeleteAsync(Guid.NewGuid(), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    private async Task<Role?> GetFromDbAsync(Guid id)
    {
        return await DbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, CancellationToken);
    }
}