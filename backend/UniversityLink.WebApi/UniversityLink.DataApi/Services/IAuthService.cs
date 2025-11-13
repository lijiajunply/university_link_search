namespace UniversityLink.DataApi.Services;

public interface IAuthService
{
    Task<string?> RequestTokenWithCredentials(string username, string password,
        CancellationToken cancellationToken = default);

    // 验证OAuth2令牌
    bool ValidateToken(string token);

    // 获取令牌中的用户名
    string? GetUsernameFromToken(string token);

    // 检查用户权限
    Task<bool> HasPermissionAsync(string username, string requiredPermission,
        CancellationToken cancellationToken = default);

    Task<OAuthTokenResponse?> GetAccessTokenAsync(string authorizationCode,
        CancellationToken cancellationToken = default);

    Task<OAuthUserInfo?> GetUserInfoAsync(string accessToken,
        CancellationToken cancellationToken = default);
}