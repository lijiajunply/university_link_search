using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityLink.Data;
using UniversityLink.DataApi.Services;

namespace UniversityLink.WebApi.Controllers;

// 请求和响应模型定义
[Serializable]
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[Serializable]
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

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
    public ActionResult<string> Authorize(string? redirectUri = null)
    {
        try
        {
            // 构建OAuth2授权URL
            var clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID") ?? "your-client-id";
            var redirectUrl = string.IsNullOrEmpty(redirectUri)
                ? (Environment.GetEnvironmentVariable("OAUTH_REDIRECT_URI") ?? "https://yourdomain.com/callback")
                : redirectUri;

            var state = Guid.NewGuid().ToString(); // 简单的state实现，实际项目中应该存储在session或缓存中

            var authorizeUrl = $"https://oauth-provider.com/SSO/authorize?" +
                               $"client_id={clientId}&" +
                               $"redirect_uri={Uri.EscapeDataString(redirectUrl)}&" +
                               $"state={state}&" +
                               $"response_type=code&" +
                               $"scope=profile email";

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
            var oauthService = (AuthService)authService; // 强制转换以访问新增的方法
            var tokenResponse = await oauthService.GetAccessTokenAsync(request.Code, cancellationToken);

            if (tokenResponse?.AccessToken == null)
            {
                return Unauthorized(new { message = "获取访问令牌失败" });
            }

            // 使用访问令牌获取用户信息
            var userInfo = await oauthService.GetUserInfoAsync(tokenResponse.AccessToken, cancellationToken);

            if (userInfo?.Sub == null)
            {
                return Unauthorized(new { message = "获取用户信息失败" });
            }

            // 检查用户是否已存在，如果不存在则创建
            try
            {
                await userService.GetUserByUsernameAsync(userInfo.Name ?? userInfo.Sub, cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                // 用户不存在，创建新用户
                var user = new UserModel
                {
                    UserName = userInfo.Name ?? userInfo.Sub,
                    UserId = userInfo.Sub,
                    Identity = userInfo.Role ?? "User",
                };

                // 创建用户（注意：实际项目中可能需要更复杂的用户创建逻辑）
                await userService.CreateUserAsync(user, "default-password", cancellationToken);
            }

            // 直接返回OAuth2访问令牌
            return Ok(new TokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                TokenType = tokenResponse.TokenType ?? "Bearer"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "OAuth2回调处理失败", error = ex.Message });
        }
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 使用Resource Owner Password Credentials流程获取OAuth2令牌
            var oauthService = (AuthService)authService;
            var token = await oauthService.RequestTokenWithCredentials(request.Username, request.Password,
                cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            // 直接返回OAuth2访问令牌
            return Ok(new TokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "登录失败", error = ex.Message });
        }
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 创建用户对象
            var user = new UserModel
            {
                UserName = request.Username,
                // 这里假设UserId会在CreateUserAsync中生成或处理
                Identity = "Member" // 默认为普通成员
            };

            // 创建用户
            await userService.CreateUserAsync(user, request.Password, cancellationToken);

            return Ok(new { message = "注册成功" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "注册失败", error = ex.Message });
        }
    }

    // POST: api/auth/refresh-token
    [HttpPost("refresh-token")]
    [Authorize]
    public ActionResult<TokenResponse> RefreshToken()
    {
        try
        {
            // 在纯OAuth2模式下，刷新令牌应该通过OAuth2提供商处理
            // 这里返回一个模拟的响应
            return Ok(new TokenResponse
            {
                AccessToken = "new-oauth2-access-token",
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "刷新令牌失败", error = ex.Message });
        }
    }

    // GET: api/auth/validate-token
    [HttpGet("validate-token")]
    [Authorize]
    public ActionResult<TokenValidationResponse> ValidateToken()
    {
        try
        {
            // 从Authorization头获取令牌
            var authHeader = HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "无效的授权头" });
            }

            var token = authHeader.Replace("Bearer ", string.Empty);

            // 验证OAuth2令牌
            var isValid = authService.ValidateToken(token);

            if (!isValid)
            {
                return Unauthorized(new { message = "令牌无效" });
            }

            // 获取令牌中的用户名和角色信息
            var username = authService.GetUsernameFromToken(token);
            // 注意：角色信息需要从OAuth2提供商获取，这里简化处理
            var role = "User"; // 默认角色
            var userId = username ?? string.Empty;

            return Ok(new TokenValidationResponse
            {
                IsValid = true,
                Username = username,
                Role = role,
                UserId = userId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "验证令牌失败", error = ex.Message });
        }
    }

    // GET: api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        try
        {
            // 对于OAuth2，服务端不需要特殊处理，客户端删除令牌即可
            // 这里可以记录用户登出日志等
            return Ok(new { message = "登出成功" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "登出失败", error = ex.Message });
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