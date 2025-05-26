using CinemaService.DTOs;
using CinemaService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CinemaService.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheatersController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        [HttpGet("get-all-in-cinema")]
        public async Task<IActionResult> GetAll(Guid cinemaId)
        {
            var rooms = await _theaterService.GetAll(cinemaId);
            return Ok(rooms);
        }

        [HttpGet("get-theater")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _theaterService.GetbyId(id);
            if(room == null) return NotFound("Theater not found");
            return Ok(room);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TheaterCreateDTO theaterCreateDTO)
        {
            try
            {
                await _theaterService.Create(theaterCreateDTO);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TheaterUpdateDTO theaterUpdateDTO)
        {
            try
            {
                await _theaterService.Update(id, theaterUpdateDTO);
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
                await _theaterService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
