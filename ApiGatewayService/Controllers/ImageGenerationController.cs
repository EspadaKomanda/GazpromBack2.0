using ApiGatewayService.Services.ImageAgregationService;
using ImageAgregationService.Exceptions.GenerateImageExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGatewayService.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageAgregationController(IImageAgregationService imageAgregationSerivce) : ControllerBase
{
    private readonly IImageAgregationService _imageAgregationSerivce = imageAgregationSerivce;

    /// <summary>
    /// Генерация изображения
    /// </summary>
    /// <response code="200">Успешная генерация</response>
    /// <response code="400">Неверные данные</response>
    [HttpPost]
    [Route("generateImage")]
    [Authorize(Roles = "User", Policy = "Access")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<ImageDto>> GenerateImage([FromBody] GenerateImageKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _imageAgregationSerivce.GenerateImage(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GenerateImageException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получение списка изображений
    /// </summary>
    /// <response code="200">Изображения получены</response>
    /// <response code="400">Неверные данные</response>
    [HttpGet]
    [Route("getImages")]
    [Authorize(Roles = "User", Policy = "Access")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<List<ImageDto>>> GetImages([FromQuery] GetImagesKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _imageAgregationSerivce.GetImageDto(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is GenerateImageException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}