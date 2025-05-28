using P2.CinemaService.DTOs;
using P2.CinemaService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CinemaService.Controllers
{
    [Route("api/screenings")]
    [ApiController]
    public class ScreeningController : ControllerBase
    {
        private readonly IScreeingService _screeningService;

        public ScreeningController(IScreeingService screeningService)
        {
            _screeningService = screeningService;
        }

        [HttpGet("get-all-in-cinema")]
        public async Task<IActionResult> GetAll(Guid cinemaId)
        {
            var screeningInCiname = await _screeningService.GetAll(cinemaId);
            return Ok(screeningInCiname);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var screening = await _screeningService.GetbyId(id);
            if (screening == null) return NotFound();
            return Ok(screening);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ScreeningCreateDTO screeningCreateDTO)
        {
            try
            {
                await _screeningService.Create(screeningCreateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ScreeningUpdateDTO screeningUpdateDTO)
        {
            try
            {
                await _screeningService.Update(id, screeningUpdateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _screeningService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
