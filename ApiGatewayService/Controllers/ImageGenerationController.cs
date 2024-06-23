using ImageAgregationService.Models.RequestModels;

using ApiGatewayService.Services.ImageAgregationService;
using ImageAgregationService.Exceptions.GenerateImageExceptions;
using ImageAgregationService.Models.DTO;
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
    [Authorize(Policy = "Access")]
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
    [Authorize(Policy = "Access")]
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
    [HttpGet]
    [Route("getLikedImages")]
    public async Task<ActionResult<string>> GetLikedImages()
    {
        try
        {
            var result = await _imageAgregationSerivce.GetLikedImages();
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
    [HttpGet]
    [Route("getPagesCount")]
    public async Task<ActionResult<int>> GetPagesCount()
    {
        try
        {
            var result = await _imageAgregationSerivce.GetPagesCount();
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
    [HttpPost]
    [Route("getSpecificImages")]
    public async Task<ActionResult<string>> GetSpecificImages([FromBody] GetImagesKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _imageAgregationSerivce.GetSpecificImages(model);
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
    [HttpPost]
    [Route("getImagesPage")]
    public async Task<ActionResult<List<ImageDto>>> GetImagesPage([FromBody] GetPage model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }        
        try
        {
            var result = await _imageAgregationSerivce.GetImagesPage(model);
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
    [HttpGet]
    [Route("getUniqueKeyWords")]
    public async Task<ActionResult<List<KeyWordDTO>>> GetUniqueKeyWords()
    {
        try
        {
            var result = await _imageAgregationSerivce.GetUniqueKeyWords();
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
    [HttpPost]
    [Route("getImagesByKeywords")]
    public async Task<ActionResult<List<ImageDto>>> GetImagesByKeywords([FromBody] GetImagesByKeywords model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }        
        try
        {
            var result = await _imageAgregationSerivce.GetImagesByKeywords(model);
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