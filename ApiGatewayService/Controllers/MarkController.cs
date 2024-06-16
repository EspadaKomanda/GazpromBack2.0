using ImageAgregationService.Exceptions.MarkExceptions;
using ImageAgregationService.Models.RequestModels.Mark;
using ImageAgregationService.Services.MarkService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class MarkController(IMarkService markService) : ControllerBase 
{
    private readonly IMarkService _markService = markService;

    /// <summary>
    /// Обновление оценки
    /// </summary>
    /// <response code="200">Оценка обновлена</response>
    /// <response code="400">Неверные данные</response>
    [HttpPatch]
    [Route("updateMark")]
    [Authorize(Roles = "User", Policy = "Access")]
    public async Task<ActionResult<bool>> UpdateMark([FromBody] UpdateMarkKafkaRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _markService.UpdateMark(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e is UpdateMarkException)
            {
                return BadRequest(e.Message);
            }
            return StatusCode(500);
        }
    }
}