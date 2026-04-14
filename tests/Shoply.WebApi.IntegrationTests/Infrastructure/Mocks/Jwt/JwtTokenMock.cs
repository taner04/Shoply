using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.Mocks.Jwt;

public enum UserRole
{
    User,
    Admin
}

public static class JwtTokenMock
{
    private const string Audience = "https://shoply-api.com";
    public const string Issuer = "https://dev-fdj2u7ky31tzfytr.eu.auth0.com/";

    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public static readonly SecurityKey SecurityKey =
        new SymmetricSecurityKey("THIS_IS_A_TEST_KEY_32_BYTES_LONG!!"u8.ToArray());

    private static readonly SigningCredentials SigningCredentials =
        new(SecurityKey, SecurityAlgorithms.HmacSha256);

    public static string CreateToken(
        string sub,
        UserRole role)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new("email", UserFactory.Email),
            new("sub", sub),
            new("aud", Audience),
            new("permissions", $"{role.ToString().ToLower()}:create"),
            new("permissions", $"{role.ToString().ToLower()}:read")
        };

        var token = new JwtSecurityToken(
            Issuer,
            Audience,
            claims,
            now,
            now.AddHours(1),
            SigningCredentials
        );

        return TokenHandler.WriteToken(token);
    }
}