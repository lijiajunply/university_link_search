using System.Text.Json;
using System.Text.Json.Serialization;

namespace UniversityLink.DataApi.Services;

public class AuthService(IUserService userService) : IAuthService
{
    private readonly HttpClient _httpClient = new();
    private readonly string _clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID") ?? "your-client-id";

    private readonly string _clientSecret =
        Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET") ?? "your-client-secret";

    private readonly string _redirectUri =
        Environment.GetEnvironmentVariable("OAUTH_REDIRECT_URI") ?? "https://yourdomain.com/callback";

    private readonly string _oauthBaseUri =
        Environment.GetEnvironmentVariable("OAUTH_BASE_URI") ?? "https://oauth-provider.com";

    // 从环境变量读取OAuth2配置

    // 使用用户名和密码直接请求令牌
    public async Task<string?> RequestTokenWithCredentials(string username, string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_oauthBaseUri}/SSO/token");

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = username,
                ["password"] = password,
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret
            };

            request.Content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

            return tokenResponse?.TryGetValue("access_token", out var value) is true ? value.ToString() : null;
        }
        catch
        {
            return null;
        }
    }

    // 验证OAuth2访问令牌
    public bool ValidateToken(string token)
    {
        try
        {
            // 直接验证OAuth2令牌
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_oauthBaseUri}/SSO/userinfo");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = _httpClient.SendAsync(request).Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // 从OAuth2令牌中获取用户名
    public string? GetUsernameFromToken(string token)
    {
        try
        {
            // 通过OAuth2提供商获取用户信息
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_oauthBaseUri}/SSO/userinfo");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = response.Content.ReadAsStringAsync().Result;
            var userInfo = JsonSerializer.Deserialize<OAuthUserInfo>(content);
            return userInfo?.Name ?? userInfo?.Sub;
        }
        catch
        {
            return null;
        }
    }

    // 检查用户权限
    public async Task<bool> HasPermissionAsync(string username, string requiredPermission,
        CancellationToken cancellationToken = default)
    {
        // 获取用户信息
        var user = await userService.GetUserByUsernameAsync(username, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // 管理员拥有所有权限
        if (user.Identity == "Admin")
        {
            return true;
        }

        // 根据角色检查权限
        return requiredPermission.ToLower() switch
        {
            "read" =>
                // 所有用户都有读取权限
                true,
            "write" or "edit" or "update" =>
                // 编辑权限需要User或更高角色
                user.Identity is "User" or "Admin",
            "delete" =>
                // 删除权限需要Admin角色
                user.Identity == "Admin",
            "admin" =>
                // 管理员操作需要Admin角色
                user.Identity == "Admin",
            _ => false
        };
    }

    // 新增：使用授权码获取访问令牌
    public async Task<OAuthTokenResponse?> GetAccessTokenAsync(string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_oauthBaseUri}/SSO/token");

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = authorizationCode,
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret,
                ["redirect_uri"] = _redirectUri
            };

            request.Content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<OAuthTokenResponse>(content);
        }
        catch
        {
            return null;
        }
    }

    // 新增：使用访问令牌获取用户信息
    public async Task<OAuthUserInfo?> GetUserInfoAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_oauthBaseUri}/SSO/userinfo");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<OAuthUserInfo>(content);
        }
        catch
        {
            return null;
        }
    }
}

// 添加用于反序列化的数据模型
[Serializable]
public class OAuthTokenResponse
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
    [JsonPropertyName("token_type")] public string? TokenType { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    [JsonPropertyName("scope")] public string? Scope { get; set; }
}

[Serializable]
public class OAuthUserInfo
{
    [JsonPropertyName("sub")] public string? Sub { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("nickname")] public string? Nickname { get; set; }
    [JsonPropertyName("academy")] public string? Academy { get; set; }
    [JsonPropertyName("class")] public string? ClassName { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("role")] public string? Role { get; set; }
    [JsonPropertyName("phone")] public string? Phone { get; set; }
}