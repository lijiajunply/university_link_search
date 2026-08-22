using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UniversityLink.Data;
using UniversityLink.DataApi.Services;

namespace UniversityLink.WebApi.Controllers;

public class JwtGenerate : IJwtGenerate
{
    public string GenerateJwtToken(OAuthUserInfo userInfo)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                        ?? throw new InvalidOperationException("缺少必需的环境变量 JWT_SECRET，无法签发令牌");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Sub ?? ""),
            new Claim(ClaimTypes.Name, userInfo.Name ?? ""),
            new Claim(ClaimTypes.Role, userInfo.Role ?? "User"),
            new Claim("userId", userInfo.Sub ?? ""),
            new Claim("userName", userInfo.Name ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: "UniversityLink",
            audience: "UniversityLinkUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}