using ApiGatewayService.Exceptions.User;
using ApiGatewayService.Models.User;
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
    public async Task<ActionResult<UserModel>> GetUserByUsername([FromQuery] string username)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.GetUserByUsername(username);

            if (result == null)
            {
                return NotFound("User not found");
            }

            var model = new UserModel
            {
                Id = result.Id,
                Username = result.Username
            };
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