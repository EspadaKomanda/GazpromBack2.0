using AuthService.Database.Models;

namespace AuthService.Services.Jwt;

public interface IJwtService
{
    Tuple<bool, string> ValidateAccessToken(string? token);
    Task<Tuple<bool, string>> ValidateRefreshToken(string? token);
}
