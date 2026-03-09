using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Test.Builders;

[BuilderFor(typeof(UpdateRoleRequest))]
public partial class UpdateRoleRequestBuilder : IBuilder<UpdateRoleRequest, UpdateRoleRequestBuilder>
{
    /// <summary>
    /// Returns a valid UpdateRoleRequest with randomized data.
    /// </summary>
    public static UpdateRoleRequestBuilder Default => new UpdateRoleRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets defaults that represent a typical, valid update operation.
    /// </summary>
    public UpdateRoleRequestBuilder WithDefaults()
    {
        return WithRandomName()
            .WithDescription("Updated role description content.");
    }

    #region Domain Helpers

    /// <summary>
    /// Generates a randomized name to simulate an edit.
    /// </summary>
    public UpdateRoleRequestBuilder WithRandomName()
    {
        var adjectives = new[] { "Modified", "Enhanced", "Legacy", "Primary" };
        var titles = new[] { "Access", "Profile", "Member", "Supervisor" };

        var name = $"{adjectives[Random.Shared.Next(adjectives.Length)]} {titles[Random.Shared.Next(titles.Length)]}";

        return WithName(name);
    }

    /// <summary>
    /// Sets the description to null, useful for testing the clearing of a field.
    /// </summary>
    public UpdateRoleRequestBuilder WithNullDescription()
    {
        return WithDescription((string?)null);
    }

    /// <summary>
    /// Sets an empty name to trigger "Name is Required" validation during an update.
    /// </summary>
    public UpdateRoleRequestBuilder WithInvalidEmptyName()
    {
        return WithName(string.Empty);
    }

    public static implicit operator UpdateRoleRequest(UpdateRoleRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}