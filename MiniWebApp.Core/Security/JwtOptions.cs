namespace MiniWebApp.Core.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string Key { get; init; } = default!;
    public int ExpiryMinutes { get; init; }

    public byte[] GetBytes()
    {
        return Convert.FromBase64String(Key);
    }
}
