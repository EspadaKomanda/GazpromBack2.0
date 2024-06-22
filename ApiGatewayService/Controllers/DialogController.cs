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
    /// <response code="200">Диалог создан</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("createDialog")]
    //[Authorize(Policy = "Access")]
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
    /// <response code="200">Сообщения получены</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getDialogs")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<List<Dialog>>> GetDialogMessages([FromQuery] GetDialogMessagesRequest model)
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
    /// <response code="200">Список диалогов получен</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getDialogsByOwnerId")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<List<Dialog>>> GetDialogsByOwnerId([FromQuery] GetDialogsByIdRequest model)
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
    /// <response code="200">Диалог удален</response>
    /// <response code="400">Неверные данные</response>
    [HttpDelete]
    [Route("deleteDialog")]
    //[Authorize(Policy = "Access")]
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