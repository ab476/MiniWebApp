using MiniWebApp.Core.Models;
using MiniWebApp.Core.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MiniWebApp.UserApi.Infrastructure.Serialization;

/// <summary>
/// Source-generated JSON serialization context for the MiniWebApp.
/// This improves performance and reduces memory overhead by avoiding runtime reflection.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(JwtUser))]
[JsonSerializable(typeof(CreateRoleRequest))]
[JsonSerializable(typeof(ClaimResponse))]
[JsonSerializable(typeof(PagedResponse<ClaimResponse>))]
// Add any other DTOs or Records you return from your API

// 1. Response Wrappers (including generic versions)
[JsonSerializable(typeof(Outcome<ClaimResponse>))]
[JsonSerializable(typeof(Outcome<PagedResponse<ClaimResponse>>))]
[JsonSerializable(typeof(PagedResponse<ClaimResponse>))]

// 2. Specific Permission DTOs
[JsonSerializable(typeof(ClaimResponse))]
[JsonSerializable(typeof(GetClaimRequest))]

// 3. Query Models (if they are bound from JSON, though PagedRequest is usually [FromQuery])
[JsonSerializable(typeof(PagedRequest))]
public partial class UserApiJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// Resolves the <see cref="JsonTypeInfo{T}"/> for a type using runtime custom converters,
    /// ignoring the standard CamelCase naming policy.
    /// </summary>
    public  JsonTypeInfo<T> GetRequiredTypeInfoForRuntimeConverter<T>()
    {
        // 1. Create options inside the method
        var options = new JsonSerializerOptions
        {
            // By setting PropertyNamingPolicy to null, we ignore CamelCase 
            // and use the property names exactly as defined in the class (PascalCase).
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // 2. Attempt to find a converter
        JsonConverter? converter = GetRuntimeConverterForType(typeof(T), options);

        if (converter is not null)
        {
            // 3. Create the metadata using the custom options
            return JsonMetadataServices.CreateValueInfo<T>(options, converter);
        }

        throw new InvalidOperationException(
            $"No custom JsonConverter found for type '{typeof(T).FullName}' using the provided options.");
    }
}

public static class Extensions
{
    public static IServiceCollection AddCustomSerialization(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            // Tells the Minimal API/Controllers to use the source-generated context
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, UserApiJsonSerializerContext.Default);
        });

        return services;
    }
}