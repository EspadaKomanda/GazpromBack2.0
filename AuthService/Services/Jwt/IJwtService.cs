using BackGazprom.Database.Models;

namespace AuthService.Services.Jwt;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    Tuple<bool, string> ValidateAccessToken(string? token);
    Task<Tuple<bool, string>> ValidateRefreshToken(string? token);
}
