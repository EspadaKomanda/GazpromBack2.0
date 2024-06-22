using ApiGatewayService.Exceptions.MessageExceptions;
using DialogService.Database.Models;
using DialogService.Models.Messages.Requests;
using DialogService.Services.MessagesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController(IMessagesService messageService) : ControllerBase
{
    private readonly IMessagesService _messageService = messageService;

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <response code="200">Сообщение отправлено</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("sendMessage")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<Message>> SendMessage([FromBody] SendMessageRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _messageService.SendMessage(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is SendMessageException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получение сообщения
    /// </summary>
    /// <response code="200">Сообщение получено</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getMessage")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<Message>> GetMessage([FromQuery] GetMessageRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _messageService.GetMessage(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GetMessageException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удаление сообщения
    /// </summary>
    /// <response code="200">Сообщение удалено</response>
    /// <response code="400">Неверные данные</response>
    [HttpDelete]
    [Route("deleteMessage")]
    //[Authorize(Policy = "Access")]
    public async Task<ActionResult<bool>> DeleteMessage([FromBody] DeleteMessageRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _messageService.DeleteMessage(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is DeleteMessageException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}