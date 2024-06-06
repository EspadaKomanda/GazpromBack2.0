using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackGazprom.Database.Models;
using BackGazprom.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Jwt;

public class JwtService(IUserRepository userRepo) : IJwtService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly string _secretKey = Environment.GetEnvironmentVariable("AUTH_JWT_SECRET") ?? "";
    private readonly string _issuer = Environment.GetEnvironmentVariable("AUTH_JWT_ISSUER") ?? "kvakvacloud";
    private readonly string _audience = Environment.GetEnvironmentVariable("AUTH_JWT_AUDIENCE") ?? "kvakvacloud";

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

    public bool ValidateAccessToken(string? token, out string? username)
    {
        try
        {
            if (token == null)
            {
                username = null;
                return false;
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

            username = validatedJwt.Claims.First(claim => claim.Type == "Username").Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == "Type").Value != "access")
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            username = null;
            return false;
        }  
    }

    public bool ValidateRefreshToken(string? token, out string? username)
    {
        try
        {
            if (token == null)
            {
                username = null;
                return false;
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

            username = validatedJwt.Claims.First(claim => claim.Type == "Username").Value;
            var passwordChangeDate = validatedJwt.Claims.First(claim => claim.Type == "PasswordChangeDate").Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == "Type").Value != "refresh")
            {
                username=null;
                return false;
            }

            // Проверка, что дата изменения пароля совпадает с фактической
            if (_userRepo.GetUserByUsername(username)!.PasswordChangeDate.ToString() != passwordChangeDate)
            {
                username=null;
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            username=null;
            return false;
        }
    }
}