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
    /// <response code="200">Шаблоны получены</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getTemplates")]
    [Authorize(Policy = "Access")]
    public async Task<ActionResult<List<TemplateDto>>> GetTemplates([FromQuery] GetTemplateKafkaRequest model)
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
    /// <response code="200">Шаблон добавлен</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("addTemplate")]
    [Authorize(Policy = "Access")]
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
    /// <response code="200">Шаблон удален</response>
    /// <response code="400">Неверные данные</response>
    [HttpDelete]
    [Route("deleteTemplate")]
    [Authorize(Policy = "Access")]
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
    /// <response code="200">Шаблон обновлен</response>
    /// <response code="400">Неверные данные</response>
    [HttpPatch]
    [Route("updateTemplate")]
    [Authorize(Policy = "Access")]
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