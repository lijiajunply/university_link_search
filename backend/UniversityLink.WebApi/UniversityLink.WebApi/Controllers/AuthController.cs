using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.Data;
using UniversityLink.DataApi.Services;

namespace UniversityLink.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IJwtGenerate generate) : ControllerBase
{
    // GET: api/auth/authorize
    [HttpGet("authorize")]
    public ActionResult Authorize()
    {
        try
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginSuccess") // 生成 /Auth/LoginLogic
            };

            // 使用OAuth中间件处理授权
            return Challenge(properties, "ExternalOAuth");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "重定向到OAuth2提供商失败", error = ex.Message });
        }
    }

    // GET: api/auth/LoginSuccess
    [HttpGet("LoginSuccess")]
    public async Task<IActionResult> LoginSuccess(CancellationToken cancellationToken = default)
    {
        try
        {
            // 注意：这里要从 Cookie 中获取，因为中间件已经把 OAuth 结果存入 Cookie 了
            var authenticateResult =
                await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized(new { message = "认证流程失败" });
            }

            // 从 Claims 中提取信息 (中间件已经在 OnCreatingTicket 中把 JSON 映射为 Claims 了)
            var claims = authenticateResult.Principal.Claims;
            var enumerable = claims as Claim[] ?? claims.ToArray();
            var sub = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // 获取 AccessToken (需要确保在 Cookie 配置中开启了保存 Token)
            var accessToken = authenticateResult.Properties?.GetTokenValue("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "获取访问令牌失败" });
            }

            // 完全信赖 OAuth2 的数据，不再查询或写入本地数据库
            // 角色映射逻辑：如果 OAuth 未返回角色或角色为 Member，则视为 User；否则视为 Admin
            var finalRole = string.IsNullOrEmpty(role) || role == "Member" ? "User" : "Admin";

            // 返回JWT格式的令牌给前端
            var token = generate.GenerateJwtToken(new OAuthUserInfo()
            {
                Sub = sub,
                Name = name,
                Role = finalRole // 保持使用原始 role，与 Program.cs 中的 Policy 匹配
            });

            return Redirect(
                $"https://start.xauat.site/callback?token={Uri.EscapeDataString(token)}&sub={Uri.EscapeDataString(sub ?? "")}&name={Uri.EscapeDataString(name ?? string.Empty)}&role={Uri.EscapeDataString(role ?? string.Empty)}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "OAuth2回调处理失败", error = ex.Message });
        }
    }
}