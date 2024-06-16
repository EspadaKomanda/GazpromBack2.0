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
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    /// <summary>
    /// Запрос на регистрацию аккаунта.
    /// </summary>
    /// <response code="200">Запрос отправлен</response>
    /// <response code="429">Кол-во попыток превышено</response>
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<bool>> Register([FromBody] AccountRegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _accountService.AccountRegister(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is RegisterException)
            {
                // FIXME: Completely uninformative service exceptions
                return Forbid(e.ToString());
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Завершение регистрации.
    /// </summary>
    /// <response code="200">Получены токены</response>
    /// <response code="401">Неверные данные входа</response>
    [HttpPost]
    [Route("finishRegistration")]
    public async Task<ActionResult<bool>> FinishRegistration([FromBody] AccountFinishRegistrationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _accountService.AccountFinishRegistration(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is FinishRegistrationException)
            {
                // FIXME: Completely uninformative service exceptions
                return Forbid(e.ToString());
            }
            return StatusCode(500);
        }
    }

}