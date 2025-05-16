using Microsoft.AspNetCore.Mvc;
using ScheduleService.DTOs;
using ScheduleService.Services.IServices;

namespace ScheduleService.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IScreeningService _screeningService;
        public SchedulesController(IScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _screeningService.GetAll();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }   

        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _screeningService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ScreeningCreateDTO screeningCreateDTO)
        {
            try
            {
                await _screeningService.Create(screeningCreateDTO);
                return Ok(new { message = "Screening created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ScreeningUpdateDTO screeningUpdateDTO)
        {
            try
            {
                await _screeningService.Update(id, screeningUpdateDTO);
                return Ok(new { message = "Screening updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _screeningService.Delete(id);
                return Ok(new { message = $"Screening code: {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
