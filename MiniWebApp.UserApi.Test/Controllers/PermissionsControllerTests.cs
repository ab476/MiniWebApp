using FluentAssertions;
using MiniWebApp.Core.Common;
using MiniWebApp.Core.Models;
using MiniWebApp.UserApi.Infrastructure.Serialization;
using MiniWebApp.UserApi.Models.Permissions;
using System.Net;
using System.Net.Http.Json;

namespace MiniWebApp.UserApi.Test.Controllers;

public class PermissionsControllerTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    const string BaseUrl = "/api/permissions";
    [Fact]
    public async Task ListPermissions_WithValidUser_ReturnsPagedData()
    {
        var client = AuthClientBuilder()
            .SystemPermissions.CanRead() 
            .Build();

        // Act
        var response = await client.GetAsync($"{BaseUrl}?pageNumber=1", ct);

        // Assert: Output
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.DeserializePagedResponseAsync<PermissionResponse>(ct);
        
        result!.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().NotBeEmpty();
    }
}
public static class HttpResponseMessageExtensions {
    static UserApiJsonSerializerContext UserApiJsonSerializer => UserApiJsonSerializerContext.Default;

    extension(HttpResponseMessage response)
    {
        public async Task<string> ReadContentAsStringAsync(CancellationToken cancellationToken = default)
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        public async Task<Outcome<T>> DeserializeResponseAsync<T>(CancellationToken ct = default)
        {
            // Rename 'jsonTypeInfo' to 'metadata' or 'typeInfo'
            var metadata = UserApiJsonSerializer.GetRequiredTypeInfoForRuntimeConverter<Outcome<T>>();

            // Rename 'result' to 'outcome' to match the domain model
            var outcome = await response.Content.ReadFromJsonAsync(metadata, ct);

            return outcome ?? throw new InvalidOperationException($"Could not decode the API response as {typeof(T).Name}.");
        }
        public Task<Outcome<PagedResponse<T>>> DeserializePagedResponseAsync<T>(CancellationToken ct = default)
        {
            return response.DeserializeResponseAsync<PagedResponse<T>>(ct);
        }
    }
}