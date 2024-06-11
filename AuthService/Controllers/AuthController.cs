// using System.Security.Claims;
// using AuthService.Models.Account.Requests;
// using AuthService.Models.Account.Responses;
// using AuthService.Services.Account;
// using AuthService.Services.Jwt;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace AuthService.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class AuthController(IJwtService jwtService, IAccountService accountService) : ControllerBase
// {
//     private readonly IJwtService _jwtService = jwtService;
//     private readonly IAccountService _accountService = accountService;

    
    
//     /// <summary>
//     /// Запрос на регистрацию нового пользователя.
//     /// </summary>
//     /// <response code="200">Получены токены</response>
//     /// <response code="429">Превышено количество попыток регистрации</response>
//     [Route("register")]
//     [HttpPost]
//     public async Task<IActionResult> Register([FromBody] AccountRegisterRequest model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }
//         return await _accountService.AccountRegister(model);
//     }

//     /// <summary>
//     /// Завершение регистрации нового пользователя.
//     /// </summary>
//     /// <response code="200">Получены токены</response>
//     /// <response code="400">Некорректные данные</response>
//     /// <response code="401">Неверный код регистрации</response>
//     [Route("finishRegistration")]
//     [HttpPost]
//     public async Task<ActionResult<AccountTokensResponse>> FinishRegistration([FromBody] AccountFinishRegistrationRequest model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }
//         return await _accountService.AccountFinishRegistration(model);
//     }

//     /// <summary>
//     /// Аутентификация пользователя.
//     /// </summary>
//     /// <response code="200">Получены токены</response>
//     /// <response code="401">Неверные данные входа</response>
//     [Route("login")]
//     [HttpPost]
//     public async Task<ActionResult<AccountTokensResponse>> Login([FromBody] AccountLoginRequest model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }
//         return await _accountService.AccountLogin(model);
//     }

//     /// <summary>
//     /// Изменение пароля пользователя.
//     /// </summary>
//     /// <response code="200">Получены токены</response>
//     /// <response code="400">Некорректные данные</response>
//     /// <response code="401">Неверные данные входа или неверный старый пароль</response>
//     [Route("changePassword")]
//     [HttpPost]
//     [Authorize(Roles = "User", Policy = "Access")]
//     public async Task<ActionResult<AccountTokensResponse>> ChangePassword([FromBody] AccountChangePasswordRequest model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

//         return await _accountService.AccountChangePassword(username, model);
//     }

//     /// <summary>
//     /// Обновление токенов.
//     /// </summary>
//     /// <response code="200">Получены токены</response>
//     /// <response code="401">Неверные данные входа</response>
//     [Route("refreshToken")]
//     [HttpPost]
//     [Authorize(Roles = "User", Policy = "Refresh")]
//     public async Task<ActionResult<AccountTokensResponse>> RefreshToken(AccountRefreshTokenRequest model)
//     {
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

//         return await _accountService.AccountRefreshToken(username, model);
//     }

// }