namespace UniversityLink.DataApi.Services;

public interface IAuthService
{
    Task<OAuthTokenResponse?> GetAccessTokenAsync(string authorizationCode,
        CancellationToken cancellationToken = default);

    Task<OAuthUserInfo?> GetUserInfoAsync(string accessToken,
        CancellationToken cancellationToken = default);
}