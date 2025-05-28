using Microsoft.AspNetCore.Mvc;
using P2.CinemaService.DTOs;
using P2.CinemaService.Services.IServices;

namespace P2.CinemaService.Controllers
{
    [Route("api/cinemas")]
    [ApiController]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var cinemas = await _cinemaService.GetAll();
            return Ok(cinemas);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cinema = await _cinemaService.GetbyId(id);
            if (cinema == null) return NotFound();
            return Ok(cinema);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CinemaCreateDTO cinemaCreateDTO)
        {
            try
            {
                await _cinemaService.Create(cinemaCreateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CinemaUpdateDTO cinemaUpdateDTO)
        {
            try
            {
                await _cinemaService.Update(id, cinemaUpdateDTO);
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
                await _cinemaService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
