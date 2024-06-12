namespace DialogService.Services.Jwt;

public interface IJwtService
{
    Tuple<bool, string> ValidateAccessToken(string? token);
}
