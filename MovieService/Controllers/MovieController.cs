using Microsoft.AspNetCore.Mvc;
using MovieService.DTOs;
using MovieService.Services.IService;

namespace MovieService.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieService.GetAll();
            return Ok(movies);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var movie = await _movieService.GetById(id);

            if(movie == null)
                return NotFound(new { message = "Movie not found" });

            return Ok(movie);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] MovieCreateDTO movieCreateDTO)
        {
            try
            {
                await _movieService.CreateAsync(movieCreateDTO);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }   
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromForm] MovieUpdateDTO movieUpdateDTO)
        {
            try
            {
                await _movieService.UpdateAsync(id, movieUpdateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("get-all")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _movieService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
