using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.Data;
using UniversityLink.DataApi.Services;

namespace UniversityLink.WebApi.Controllers;

[Serializable]
public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
}

// 添加OAuth相关的响应模型
[Serializable]
public class OAuthCallbackRequest
{
    public string Code { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    // GET: api/auth/authorize
    [HttpGet("authorize")]
    [AllowAnonymous]
    public ActionResult<string> Authorize()
    {
        try
        {
            // 构建OAuth2授权URL
            var clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID") ?? "your-client-id";
            var redirectUrl = Environment.GetEnvironmentVariable("OAUTH_REDIRECT_URI") ??
                              "https://start.xauat.site/callback";

            var state = Guid.NewGuid().ToString(); // 简单的state实现，实际项目中应该存储在session或缓存中

            var authorizeUrl = $"https://api.xauat.site/SSO/authorize?" +
                               $"client_id={clientId}&" +
                               $"redirect_uri={Uri.EscapeDataString(redirectUrl)}&" +
                               $"state={state}&" +
                               $"response_type=code&" +
                               $"scope=profile email openid read";

            // 重定向到OAuth2提供商
            return Redirect(authorizeUrl);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "重定向到OAuth2提供商失败", error = ex.Message });
        }
    }

    // GET: api/auth/callback
    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Callback([FromQuery] OAuthCallbackRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { message = "授权码不能为空" });
            }

            // 使用授权码获取访问令牌
            var tokenResponse = await authService.GetAccessTokenAsync(request.Code, cancellationToken);

            if (tokenResponse?.AccessToken == null)
            {
                return Unauthorized(new { message = "获取访问令牌失败" });
            }

            // 使用访问令牌获取用户信息
            var userInfo = await authService.GetUserInfoAsync(tokenResponse.AccessToken, cancellationToken);

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

            // 直接返回OAuth2访问令牌
            return Redirect(
                $"https://start.xauat.site/callback?token={tokenResponse.AccessToken}&sub={userInfo.Sub}&name={userInfo.Name}&role={userInfo.Role}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "OAuth2回调处理失败", error = ex.Message });
        }
    }
}

// 添加TokenValidationResponse类定义
[Serializable]
public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? UserId { get; set; }
}