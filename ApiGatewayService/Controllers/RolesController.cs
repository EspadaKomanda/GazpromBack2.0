using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Exceptions.RoleExceptions;
using UserService.Models.Roles.Requests;
using UserService.Services.Roles;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class RolesController(IRolesService rolesService) : ControllerBase
{
    private readonly IRolesService _rolesService = rolesService;

    /// <summary>
    /// Возвращает список ролей
    /// </summary>
    /// <response code="200">Роли получены</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getRoles")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<List<Role>>> GetRoles()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _rolesService.GetRoles();
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GetRolesException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Добавляет роль
    /// </summary>
    /// <response code="200">Роль добавлена</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("addRole")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<bool>> CreateRole([FromBody] RoleCreateRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _rolesService.CreateRole(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is CreateRoleException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновляет роль
    /// </summary>
    /// <response code="200">Роль обновлена</response>
    /// <response code="400">Неверные данные</response>
    [HttpPatch]
    [Route("updateRole")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<bool>> UpdateRole([FromBody] RoleUpdateRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _rolesService.UpdateRole(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is UpdateRoleException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удаляет роль
    /// </summary>
    /// <response code="200">Роль удалена</response>
    /// <response code="400">Неверные данные</response>
    [HttpDelete]
    [Route("deleteRole")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<bool>> DeleteRole([FromBody] RoleDeleteRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _rolesService.DeleteRole(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is DeleteRoleException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}