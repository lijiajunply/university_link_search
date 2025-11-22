using System.Text.Json;
using System.Text.Json.Serialization;

namespace UniversityLink.DataApi.Services;

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

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient = new();
    private readonly string _clientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID") ?? "your-client-id";

    private readonly string _clientSecret =
        Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET") ?? "your-client-secret";

    private readonly string _redirectUri =
        Environment.GetEnvironmentVariable("OAUTH_REDIRECT_URI") ?? "https://link.xauat.site/auth/callback";

    private readonly string _oauthBaseUri =
        Environment.GetEnvironmentVariable("OAUTH_BASE_URI") ?? "https://api.xauat.site";

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