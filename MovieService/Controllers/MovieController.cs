using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieService.DTOs;
using MovieService.Services.IService;
using System.Security.Claims;

namespace MovieService.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MovieController(IMovieService movieService, IHttpContextAccessor httpContextAccessor)
        {
            _movieService = movieService;
            _httpContextAccessor = httpContextAccessor;
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

            if (movie == null)
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
            catch (Exception ex)
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

        [HttpDelete("delete")]
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

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult GetUser()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user?.FindFirst(ClaimTypes.Email)?.Value;
                var role = user?.FindFirst(ClaimTypes.Role)?.Value;
                return Ok(new { userId, email, role });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
