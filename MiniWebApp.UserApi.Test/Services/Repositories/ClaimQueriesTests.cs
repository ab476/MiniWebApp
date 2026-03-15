using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MiniWebApp.UserApi.Test.Builders.Claims;

namespace MiniWebApp.UserApi.Test.Services.Repositories;

public class ClaimQueriesTests(PostgresContainerFixture fixture)
    : IntegrationTestBase(fixture)
{
    private IClaimQueries Queries => GetService<IClaimQueries>();

    [Fact]
    public async Task GetClaimAsync_ByCode_IsCaseInsensitiveAndTrimmed()
    {
        // Arrange
        var permission = await SeedClaimAsync(b => b.WithCode("user.write"));

        // Testing that the query handles messy input by normalizing to the Natural Key
        var request = new GetClaimRequest("  USER.WRITE  ");

        // Act
        var result = await Queries.GetClaimAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ClaimCode.Should().Be("user.write");
    }

    [Fact]
    public async Task GetClaimAsync_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new GetClaimRequest("non.existent.claim");

        // Act
        var result = await Queries.GetClaimAsync(request, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #region ListClaims Tests

    [Fact]
    public async Task ListClaims_ReturnsAllClaimsOrderedByCode()
    {
        // Arrange
        await SeedClaimsAsync(
            b => b.WithCode("b.claim"),
            b => b.WithCode("a.claim"),
            b => b.WithCode("c.claim")
        );

        // Act
        var result = await Queries.ListClaimsAsync(CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeInAscendingOrder(p => p.ClaimCode);
        result.Value!.Select(x => x.ClaimCode).Should().Contain(["a.claim", "b.claim", "c.claim"]);
    }

    #endregion

    #region Helpers

    private async Task<AppClaim> SeedClaimAsync(Action<ClaimBuilder>? configure = null)
    {
        var builder = ClaimBuilder.New(); // Using New() to avoid shared Default state in loops
        configure?.Invoke(builder);
        var claim = builder.Build();

        await DbContext.Claims.AddAsync(claim, CancellationToken);
        await DbContext.SaveChangesAsync(CancellationToken);

        // Clear tracker to ensure subsequent queries hit the database, not the cache
        DbContext.ChangeTracker.Clear();

        return claim;
    }

    private async Task SeedClaimsAsync(params Action<ClaimBuilder>[] configs)
    {
        foreach (var config in configs)
            await SeedClaimAsync(config);
    }

    #endregion
}