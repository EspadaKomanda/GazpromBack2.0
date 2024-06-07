using System.Security.Claims;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Services.Account;
using AuthService.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtService jwtService, IAccountService accountService) : ControllerBase
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IAccountService _accountService = accountService;

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await _accountService.AccountRegister(model);
    }

    [Route("finishRegistration")]
    [HttpPost]
    public async Task<ActionResult<AccountTokensResponse>> FinishRegistration([FromBody] AccountFinishRegistrationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await _accountService.AccountFinishRegistration(model);
    }

    [Route("login")]
    [HttpPost]
    public async Task<ActionResult<AccountTokensResponse>> Login([FromBody] AccountLoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await _accountService.AccountLogin(model);
    }

    [Route("changePassword")]
    [HttpPost]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<AccountTokensResponse>> ChangePassword([FromBody] AccountChangePasswordRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

        return await _accountService.AccountChangePassword(username, model);
    }

    [Route("refreshToken")]
    [HttpPost]
    [Authorize(Roles = "User", Policy = "Refresh")]
    public async Task<ActionResult<AccountTokensResponse>> RefreshToken(AccountRefreshTokenRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

        return await _accountService.AccountRefreshToken(username, model);
    }

}