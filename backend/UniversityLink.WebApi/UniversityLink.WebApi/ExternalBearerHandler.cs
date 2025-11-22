using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace UniversityLink.WebApi;

public class ExternalBearerHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 检查是否有Authorization头部
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // 检查是否是Bearer令牌
        if (!authHeader.ToString().StartsWith("Bearer "))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authHeader.ToString()["Bearer ".Length..].Trim();

        // 创建一个简单的ClaimsPrincipal，因为我们信任外部OAuth提供商已经验证了令牌
        var claims = new List<Claim>
        {
            new Claim("access_token", token)
        };

        // 尝试解析JWT令牌以提取声明（如果它是有效的JWT）
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                claims.AddRange(jwtToken.Claims);
            }
        }
        catch
        {
            // 如果令牌不是有效的JWT，我们仍然可以使用它作为不透明令牌
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}