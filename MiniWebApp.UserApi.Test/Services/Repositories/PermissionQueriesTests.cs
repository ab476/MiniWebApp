using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Permissions;
using MiniWebApp.UserApi.Test.Builders;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class PermissionQueriesTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private IPermissionQueries Queries => GetService<IPermissionQueries>();

    

    [Fact]
    public async Task GetPermissionAsync_WhenNoIdOrCodeProvided_ReturnsBadRequest()
    {
        // Arrange: Use builder to create an empty/invalid request
        var request = GetPermissionRequestBuilder.Default
            .WithId(() => null)
            .WithCode(() => null)
            .Build();

        // Act
        var result = await Queries.GetPermissionAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetPermissionAsync_ByValidId_ReturnsPermission()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        await SeedPermissionAsync(b => b.WithId(targetId));

        var request = GetPermissionRequestBuilder.Default
            .WithId(targetId)
            .Build();

        // Act
        var result = await Queries.GetPermissionAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(targetId);
    }

    [Fact]
    public async Task GetPermissionAsync_ByCode_IsCaseInsensitive()
    {
        // Arrange
        var permission = await SeedPermissionAsync(b => b.WithRandomCode());

        var request = GetPermissionRequestBuilder.Default
            .WithCode($"  {permission.Code.ToUpper()}  ")
            .WithId(() => null) 
            .Build();

        // Act
        var result = await Queries.GetPermissionAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Code.Should().Be(permission.Code.ToLowerInvariant());
    }

    
    [Fact]
    public async Task GetPermissionAsync_ByCode_IsCaseInsensitiveAndTrimmed()
    {
        // Arrange
        await SeedPermissionAsync(b => b.WithCode("user.write"));
        var request = new GetPermissionRequest { Code = "  USER.WRITE  " };

        // Act
        var result = await Queries.GetPermissionAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Code.Should().Be("user.write");
    }

    [Fact]
    public async Task GetPermissionAsync_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new GetPermissionRequest { Code = "non.existent" };

        // Act
        var result = await Queries.GetPermissionAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }


    #region ListPermissions Tests

    [Fact]
    public async Task ListPermissions_ReturnsAllPermissionsOrderedByCode()
    {
        // Arrange
        await SeedPermissionsAsync(
            b => b.WithRandomCode(),
            b => b.WithRandomCode(),
            b => b.WithRandomCode()
        );

        // Act
        var result = await Queries.ListPermissions(CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeInAscendingOrder(p => p.Code);
        
    }

    #endregion

    #region Helpers

    private async Task<Permission> SeedPermissionAsync(Action<PermissionBuilder>? configure = null)
    {
        var builder = PermissionBuilder.Default;
        configure?.Invoke(builder);
        var permission = builder.Build();

        await DbContext.Permissions.AddAsync(permission, CancellationToken);
        await DbContext.SaveChangesAsync(CancellationToken);
        DbContext.ChangeTracker.Clear();

        return permission;
    }

    private async Task SeedPermissionsAsync(params Action<PermissionBuilder>[] configs)
    {
        foreach (var config in configs) await SeedPermissionAsync(config);
    }

    #endregion
}
