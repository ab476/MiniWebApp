using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniWebApp.Core.Auth;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Roles;
using System.Net;
using System.Net.Http.Json;

namespace MiniWebApp.UserApi.Test.Controllers;

public sealed class RolesControllerIntegrationTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private const string BaseUrl = "/api/roles";

    [Fact]
    public async Task GetById_Should_Return_401_When_Unauthorized()
    {
        var client = GetAnonymousClient();

        var response = await client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_Should_Return_403_When_Missing_Permission()
    {
        var client = AuthClientBuilder().Build();

        var response = await client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_Should_Return_200_When_Valid_And_Authorized()
    {
        var roleId = await CreateRoleAsync();

        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        var response = await client.GetAsync($"{BaseUrl}/{roleId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<RoleResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().Be(roleId);
    }
    [Fact]
    public async Task GetById_Should_Return_404_When_Not_Found()
    {
        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        var response = await client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task GetById_Should_Return_400_When_Invalid_Guid()
    {
        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/invalid-guid");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_Return_401_When_Unauthorized()
    {
        var client = GetAnonymousClient();

        var response = await client.PostAsJsonAsync(BaseUrl, new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Should_Return_403_When_Missing_Write_Permission()
    {
        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        var response = await client.PostAsJsonAsync(BaseUrl, new
        {
            Name = "Admin",
            TenantId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Invalid_Input()
    {
        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Write)
            .Build();

        var response = await client.PostAsJsonAsync(BaseUrl, new
        {
            Name = "",
            TenantId = Guid.Empty
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_Return_201_When_Valid()
    {
        var tenantId = await CreateTenantAsync();

        var client = AuthClientBuilder()
            .WithTenant(tenantId)
            .WithPermissions(
                AppPermissions.Roles.Write,
                AppPermissions.Roles.Read)
            .Build();

        var response = await client.PostAsJsonAsync(BaseUrl, new
        {
            Name = "Admin",
            TenantId = tenantId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<RoleResponse>();

        result.Should().NotBeNull();
        result!.Name.Should().Be("Admin");
    }

    [Fact]
    public async Task Create_Should_Handle_SQL_Injection_Safely()
    {
        var tenantId = await CreateTenantAsync();

        var client = AuthClientBuilder()
            .WithTenant(tenantId)
            .WithPermissions(AppPermissions.Roles.Write)
            .Build();

        var response = await client.PostAsJsonAsync(BaseUrl, new
        {
            Name = "'; DROP TABLE Roles; --",
            TenantId = tenantId
        });

        response.StatusCode.Should()
            .BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        var roles = await db.Roles.ToListAsync();

        roles.Should().NotBeNull(); // Table still accessible
    }

    [Fact]
    public async Task Create_Should_Handle_XSS_Safely()
    {
        var tenantId = await CreateTenantAsync();

        var client = AuthClientBuilder()
            .WithTenant(tenantId)
            .WithPermissions(AppPermissions.Roles.Write)
            .Build();

        var response = await client.PostAsJsonAsync(BaseUrl, new
        {
            Name = "<script>alert(1)</script>",
            TenantId = tenantId
        });

        response.StatusCode.Should()
            .BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetPaged_Should_Return_200_When_Valid()
    {
        var tenantId = await CreateTenantWithRolesAsync();

        var client = AuthClientBuilder()
            .WithTenant(tenantId)
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        var response = await client.GetAsync(
            $"{BaseUrl}?tenantId={tenantId}&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<RoleResponse>>();

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Update_Should_Return_403_When_Missing_Write_Permission()
    {
        var roleId = await CreateRoleAsync();

        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Read)
            .Build();

        var response = await client.PutAsJsonAsync(
            $"{BaseUrl}/{roleId}",
            new { Name = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_Should_Return_200_When_Valid()
    {
        var roleId = await CreateRoleAsync();

        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Write)
            .Build();

        var response = await client.PutAsJsonAsync(
            $"{BaseUrl}/{roleId}",
            new { Name = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Should_Return_403_When_Missing_Manage_Permission()
    {
        var roleId = await CreateRoleAsync();

        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Write)
            .Build();

        var response = await client.DeleteAsync($"{BaseUrl}/{roleId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Should_Return_200_When_Valid()
    {
        var roleId = await CreateRoleAsync();

        var client = AuthClientBuilder()
            .WithPermissions(AppPermissions.Roles.Manage)
            .Build();

        var response = await client.DeleteAsync($"{BaseUrl}/{roleId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
