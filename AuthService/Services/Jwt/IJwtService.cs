using BackGazprom.Database.Models;

namespace AuthService.Services.Jwt;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    bool ValidateAccessToken(string? token);
    Task<bool> ValidateRefreshToken(string? token);
}
