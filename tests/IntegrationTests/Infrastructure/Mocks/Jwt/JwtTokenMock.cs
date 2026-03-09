using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IntegrationTests.Factories;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Infrastructure.Mocks.Jwt;

public static class JwtTokenMock
{
    private const string Audience = "https://shoply-api.com";
    public const string Issuer = "https://dev-fdj2u7ky31tzfytr.eu.auth0.com/";
    private const string ClientId = "fnmvYdim43pToNYAA1fmUZkGQIcmdovm";
    private const string RolesClaim = "https://shoply-api/roles";

    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public static readonly SecurityKey SecurityKey =
        new SymmetricSecurityKey("THIS_IS_A_TEST_KEY_32_BYTES_LONG!!"u8.ToArray());

    private static readonly SigningCredentials SigningCredentials =
        new(SecurityKey, SecurityAlgorithms.HmacSha256);

    public static string CreateToken(
        string sub,
        string role)
    {
        var now = DateTime.UtcNow;

        var iatUnix = ToUnix(now);
        var expUnix = ToUnix(now.AddHours(1));

        var claims = new[]
        {
            new Claim("email", UserFactory.Email),
            new Claim("sub", sub),
            new Claim(RolesClaim, role),
            new Claim("scope", "openid profile email"),
            new Claim("azp", ClientId),
            new Claim("iat", iatUnix.ToString(), ClaimValueTypes.Integer64),
            new Claim("exp", expUnix.ToString(), ClaimValueTypes.Integer64),
            new Claim("aud", Audience),
            new Claim("aud", $"{Issuer}userinfo")
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

    private static long ToUnix(
        DateTime time)
    {
        return new DateTimeOffset(time).ToUnixTimeSeconds();
    }
}