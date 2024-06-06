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

    public bool ValidateAccessToken(string? token)
    {
        try
        {
            if (token == null)
            {
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

            var username = validatedJwt.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value;

            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Access")
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }  
    }

    public async Task<bool> ValidateRefreshToken(string? token)
    {
        try
        {
            if (token == null)
            {
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

            var passwordChangeDate = validatedJwt.Claims.First(claim => claim.Type == "PasswordChangeDate").Value;
            var username = validatedJwt.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            // Проверка типа токена
            if (validatedJwt.Claims.First(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value != "Refresh")
            {
                return false;
            }

            var user = await _userRepo.GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }

            // Проверка, что дата изменения пароля совпадает с фактической
            if (user.PasswordChangeDate.ToString() != passwordChangeDate)
            {
                username=null;
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}