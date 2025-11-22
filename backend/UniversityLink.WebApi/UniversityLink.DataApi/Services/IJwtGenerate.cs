using UniversityLink.Data;

namespace UniversityLink.DataApi.Services;

public interface IJwtGenerate
{
    string GenerateJwtToken(OAuthUserInfo userInfo);
}