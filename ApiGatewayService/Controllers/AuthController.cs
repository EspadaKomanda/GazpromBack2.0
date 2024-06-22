using System.Security.Claims;
using ApiGatewayService.Exceptions.User;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Services.Account;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Вход в аккаунт при помощи пароля.
    /// </summary>
    /// <response code="200">Получены токены</response>
    /// <response code="401">Неверные данные входа</response>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AccountTokensResponse>> Login([FromBody] AccountLoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.AccountLogin(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is LoginException)
            {
                return Unauthorized();
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновление токена.
    /// </summary>
    /// <response code="200">Получены токены</response>
    /// <response code="401">Неверный refresh токен</response>
    [HttpGet]
    //[Route("refreshToken")]
    public async Task<ActionResult<AccountTokensResponse>> RefreshToken([FromQuery] AccountRefreshTokenRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authService.AccountRefreshToken(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is LoginException)
            {
                return Unauthorized();
            }
            return StatusCode(500);
        }
    }

}