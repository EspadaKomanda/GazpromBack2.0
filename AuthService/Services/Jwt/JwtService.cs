using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Database.Models;
using AuthService.Services.UserService;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IUserService _userService;
    private readonly ILogger<JwtService> _logger;
    public JwtService(ILogger<JwtService> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;

        _issuer = Environment.GetEnvironmentVariable("AUTH_JWT_ISSUER") ?? "kvakvacloud";
        _audience = Environment.GetEnvironmentVariable("AUTH_JWT_AUDIENCE") ?? "kvakvacloud";

        _secretKey = Environment.GetEnvironmentVariable("AUTH_JWT_SECRET")!;
        if (_secretKey == null)
        {
            throw new Exception("Missing AUTH_JWT_SECRET environment variable");
        }
    }

    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the access token is generated.</param>
    /// <returns>The generated access token as a string.</returns>
    public string GenerateAccessToken(User user)
    {  
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_secretKey);
      
      
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(new[]
            { 
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                new Claim(ClaimTypes.AuthenticationMethod, "Access")
            }),
            Expires = DateTime.UtcNow.AddMinutes(50),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a refresh token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the refresh token is generated.</param>
    /// <returns>The generated refresh token as a string.</returns>
    public string GenerateRefreshToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(new[] 
            {
                new Claim("PasswordChangeDate", user.PasswordChangeDate.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                new Claim(ClaimTypes.AuthenticationMethod, "Refresh")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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
                _logger.LogWarning("JwtService: No access token string provided");
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

            var username = validatedJwt.Claims.First(claim => claim.Type == "unique_name").Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Access")
            {
                _logger.LogWarning("JwtService: the token was not a access token");
                return new (false, "");
            }

            return new (true, username);
        }
        catch (Exception e)
        {
            _logger.LogWarning("JwtService: Token invalidated due to exception:\n{Exception}", e);
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
        try
        {
            if (token == null)
            {
                _logger.LogWarning("JwtService: No refresh token string provided");
                return new (false, "");;
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

            var passwordChangeDate = validatedJwt.Claims.First(claim => claim.Type == "PasswordChangeDate").Value;
            var username = validatedJwt.Claims.First(claim => claim.Type == "unique_name").Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Refresh")
            {
                _logger.LogWarning("JwtService: the token was not a refresh token");
                return new (false, "");
            }

            var user = await _userService.GetUserByUsername(username);
            if (user == null)
            {
                return new (false, "");
            }

            // Проверка, что дата изменения пароля совпадает с фактической
            if (user.PasswordChangeDate.ToString() != passwordChangeDate)
            {
                username=null;
                _logger.LogWarning("JwtService: the password change date was not equal to the one in the token\n1. {1}\n2. {2}", passwordChangeDate, user.PasswordChangeDate.ToString());
                return new (false, "");
            }

            return new (true, username);
        }
        catch (Exception e)
        {
            _logger.LogWarning("JwtService: Token invalidated due to exception:\n{Exception}", e);
            return new (false, "");
        }
    }
}