using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Database.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Jwt;

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

    /// <summary>
    /// Validates the access token and extracts the username from it if valid.
    /// Returns a tuple indicating whether the token is valid and the extracted username.
    /// </summary>
    /// <param name="token">The access token to validate.</param>
    /// <returns>A tuple indicating whether the token is valid and the extracted username.</returns>
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

    /// <summary>
    /// Validates the refresh token and extracts the username from it if valid.
    /// Returns a tuple indicating whether the token is valid and the extracted username.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <returns>A tuple indicating whether the token is valid and the extracted username.</returns>
    public async Task<Tuple<bool, string>> ValidateRefreshToken(string? token)
    {
        // TODO: Auth service needs a method to validate refresh tokens
        throw new NotImplementedException();
    }
}