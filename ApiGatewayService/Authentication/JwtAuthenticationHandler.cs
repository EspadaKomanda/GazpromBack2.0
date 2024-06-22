using System.Security.Claims;
using System.Text.Encodings.Web;
using AuthService.Services.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthService.Authentication;

public class JwtAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IJwtService jwtService, ILogger<JwtAuthenticationHandler> logger) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<JwtAuthenticationHandler> _logger = logger;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            // Extract the token from the request headers or query parameters
            string? token = Request.Headers.Authorization;

            if (token == null)
            {
                _logger.LogWarning("Auth: no authorization header.");
                return AuthenticateResult.Fail("No token provided.");
            }
    
            // Validate the token based on its type
            if (token.StartsWith("Access "))
            {
                // Access token verification
                string accessToken = token["Access ".Length..];
                var validationResult = _jwtService.ValidateAccessToken(accessToken);
                if (validationResult.Item1)
                {
                    // Create a ClaimsIdentity with the user information
                    var claims = new[] 
                    {
                        new Claim(ClaimTypes.Name, validationResult.Item2!),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                        new Claim(ClaimTypes.AuthenticationMethod, "Access")
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
    
                    return AuthenticateResult.Success(ticket);
                }
                _logger.LogWarning("Auth: token didn't pass validation in AuthService");
            }
            else if (token.StartsWith("Refresh "))
            {
                // Refresh token verification
                string refreshToken = token["Refresh ".Length..];
                var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);
                if (validationResult.Item1)
                {
                    // Create a ClaimsIdentity with the user information
                    var claims = new[] 
                    {
                        new Claim(ClaimTypes.Name, validationResult.Item2!),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType,"User"),
                        new Claim(ClaimTypes.AuthenticationMethod, "Refresh")
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
    
                    return AuthenticateResult.Success(ticket);
                }
                _logger.LogWarning("Auth: token didn't pass validation in AuthService");
            }
    
            return AuthenticateResult.Fail("Invalid token");
        }
        catch (Exception e)
        {
            _logger.LogWarning("Auth: token was invalidated due to error in API Gateway:\n{Error}", e);
            return AuthenticateResult.Fail("Invalid token");
        }
    }
}
