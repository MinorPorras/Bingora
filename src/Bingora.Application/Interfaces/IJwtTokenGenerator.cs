using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime AccessTokenExpiration, string RefreshToken, DateTime RefreshTokenExpiration)
        GenerateToken(User user);
}