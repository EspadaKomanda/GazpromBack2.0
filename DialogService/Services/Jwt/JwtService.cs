using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace DialogService.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ILogger<JwtService> _logger;
    public JwtService(ILogger<JwtService> logger)
    {
        _logger = logger;

        _issuer = Environment.GetEnvironmentVariable("AUTH_JWT_ISSUER") ?? "kvakvacloud";
        _audience = Environment.GetEnvironmentVariable("AUTH_JWT_AUDIENCE") ?? "kvakvacloud";

        _secretKey = Environment.GetEnvironmentVariable("AUTH_JWT_SECRET")!;
        if (_secretKey == null)
        {
            throw new Exception("Missing AUTH_JWT_SECRET environment variable");
        }
    }

    public Tuple<bool, string> ValidateAccessToken(string? token)
    {
        try
        {
            if (token == null)
            {
                return new (false, "");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_secretKey);
    
            TokenValidationParameters validationParameters = new() {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };
    
            // Валидация токена
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            JwtSecurityToken validatedJwt = (JwtSecurityToken)validatedToken;

            var username = validatedJwt.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Access")
            {
                return new (false, "");
            }

            return new (true, username);
        }
        catch (Exception)
        {
            return new (false, "");;
        }  
    }
}