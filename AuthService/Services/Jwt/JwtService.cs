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
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                new Claim(ClaimTypes.AuthenticationMethod, "Access")
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

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
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                new Claim(ClaimTypes.AuthenticationMethod, "Refresh")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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

    public async Task<Tuple<bool, string>> ValidateRefreshToken(string? token)
    {
        try
        {
            if (token == null)
            {
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
            var username = validatedJwt.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Refresh")
            {
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
                return new (false, "");
            }

            return new (true, username);
        }
        catch (Exception)
        {
            return new (false, "");
        }
    }
}