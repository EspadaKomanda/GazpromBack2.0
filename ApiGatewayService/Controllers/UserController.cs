using ApiGatewayService.Exceptions.User;
using AuthService.Database.Models;
using Microsoft.AspNetCore.Mvc;
using UserService.Services.UserInfoService;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Поиск пользователя по имени
    /// </summary>
    /// <response code="200">Пользователь найден</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet]
    [Route("getUserByUsername")]
    public async Task<ActionResult<User>> GetUserByUsername([FromQuery] string username)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.GetUserByUsername(username);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is UserNotFoundException)
            {
                return NotFound();
            }
            return StatusCode(500);
        }
    }
}