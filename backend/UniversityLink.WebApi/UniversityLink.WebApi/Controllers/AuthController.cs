using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.Data;
using UniversityLink.DataApi.Services;

namespace UniversityLink.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService, IUserService userService, IJwtGenerate generate) : ControllerBase
{
    // GET: api/auth/authorize
    [HttpGet("authorize")]
    public ActionResult Authorize()
    {
        try
        {
            // 使用OAuth中间件处理授权
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "ExternalOAuth");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "重定向到OAuth2提供商失败", error = ex.Message });
        }
    }

    // GET: api/auth/callback
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken = default)
    {
        try
        {
            // 通过外部OAuth获取用户信息
            var authenticateResult = await HttpContext.AuthenticateAsync("ExternalOAuth");

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized(new { message = "OAuth2认证失败" });
            }

            var accessToken = authenticateResult.Properties?.GetTokenValue("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "获取访问令牌失败" });
            }

            // 使用访问令牌获取用户信息
            var userInfo = await authService.GetUserInfoAsync(accessToken, cancellationToken);

            if (string.IsNullOrEmpty(userInfo?.Sub))
            {
                return Unauthorized(new { message = "获取用户信息失败" });
            }

            // 检查用户是否已存在，如果不存在则创建
            var existingUser =
                await userService.GetUserByUsernameAsync(userInfo.Name ?? userInfo.Sub, cancellationToken);
            if (existingUser != null)
            {
                // 用户已存在，更新用户信息（注意：实际项目中可能需要更复杂的用户更新逻辑）
                existingUser.UserName = userInfo.Name ?? userInfo.Sub;
                existingUser.Identity =
                    string.IsNullOrEmpty(userInfo.Role) || userInfo.Role == "Member" ? "User" : "Admin";
                await userService.UpdateUserAsync(existingUser, cancellationToken);
            }
            else
            {
                // 用户不存在，创建新用户
                var user = new UserModel
                {
                    UserName = userInfo.Name ?? userInfo.Sub,
                    UserId = userInfo.Sub,
                    Identity = string.IsNullOrEmpty(userInfo.Role) || userInfo.Role == "Member" ? "User" : "Admin"
                };

                // 创建用户（注意：实际项目中可能需要更复杂的用户创建逻辑）
                await userService.CreateUserAsync(user, "default-password", cancellationToken);
            }

            // 返回JWT格式的令牌给前端
            var token = generate.GenerateJwtToken(userInfo);

            return Redirect(
                $"https://start.xauat.site/callback?token={Uri.EscapeDataString(token)}&sub={Uri.EscapeDataString(userInfo.Sub)}&name={Uri.EscapeDataString(userInfo.Name ?? string.Empty)}&role={Uri.EscapeDataString(userInfo.Role ?? string.Empty)}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "OAuth2回调处理失败", error = ex.Message });
        }
    }
}