using System.Text.Json.Serialization;

namespace UniversityLink.Data;

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