namespace MiniWebApp.Core.Security;

public interface IJwtTokenGenerator
{
    string Generate(JwtUser user);
}