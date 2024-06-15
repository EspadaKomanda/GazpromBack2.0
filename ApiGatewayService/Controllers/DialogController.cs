using ApiGatewayService.Exceptions.DialogExceptions;
using DialogService.Database.Models;
using DialogService.Models.Requests;
using DialogService.Services.DialogsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class DialogController(IDialogsService dialogService) : ControllerBase
{
    private readonly IDialogsService _dialogService = dialogService;

    /// <summary>q
    /// Создание диалога
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("createDialog")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<Dialog>> CreateDialog([FromBody] CreateDialogRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _dialogService.CreateDialog(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is CreateDialogException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получение сообщений в диалоге
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("getDialogs")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<List<Dialog>>> GetDialogMessages([FromBody] GetDialogMessagesRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _dialogService.GetDialogMessages(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GetDialogMessagesException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получение списка диалогов пользователя
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("getDialogsByOwnerId")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<List<Dialog>>> GetDialogsByOwnerId([FromBody] GetDialogsByIdRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _dialogService.GetDialogsByOwnerId(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GetDialogsByOwnerIdException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удаление диалога
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("deleteDialog")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<bool>> DeleteDialog([FromBody] DeleteDialogRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _dialogService.DeleteDialog(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is DeleteDialogException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}