using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bingora.Application.Interfaces;
using Bingora.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bingora.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string AccessToken, DateTime AccessTokenExpiration, string RefreshToken, DateTime RefreshTokenExpiration)
        GenerateToken(User user)
    {
        var jwtSetting = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationInMinutes = int.Parse(jwtSetting["ExpirationInMinutes"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);
        var token = new JwtSecurityToken(
            issuer: jwtSetting["Issuer"],
            audience: jwtSetting["Audience"],
            claims: claims,
            expires: accessTokenExpiration,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        return (accessToken, accessTokenExpiration, refreshToken, refreshTokenExpiration);
    }
}