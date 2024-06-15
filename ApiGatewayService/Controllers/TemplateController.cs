using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Services.TemplateService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class TemplateController(ITemplateService templateService) : ControllerBase
{
    private readonly ITemplateService _templateService = templateService;

    /// <summary>
    /// Возвращает список шаблонов
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("getTemplates")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<List<TemplateDto>>> GetTemplates([FromBody] GetTemplateKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _templateService.GetTemplates(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GetTemplateException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Добавляет шаблон
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("addTemplate")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<bool>> AddTemplate([FromBody] TemplateDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _templateService.AddTemplate(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is AddTemplateException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удаляет шаблон
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("deleteTemplate")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<bool>> DeleteTemplate([FromBody] DeleteTemplateKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _templateService.DeleteTemplate(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is DeleteTemplateException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновляет шаблон
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("updateTemplate")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<bool>> UpdateTemplate([FromBody] UpdateTemplateKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _templateService.UpdateTemplate(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is UpdateTemplateException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}