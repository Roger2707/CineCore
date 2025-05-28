using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieService.DTOs;
using MovieService.Services.IService;
using System.Security.Claims;
using static Shared.Extensions.DynamicQueries.QueryableExtensions;

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
        public async Task<IActionResult> GetAll([FromBody] MovieParams requestParmas = null)
        {
            try
            {
                var config = new FilterConfiguration()
                           .AddRule("Genres", ComparisonOperator.ContainsAny)
                           .AddRule("DurationMinutes", ComparisonOperator.LessThanOrEqual);

                var parameters = new DynamicQueryRequest
                {
                    SortFields = new List<SortField>() { new SortField { Field = "DurationMinutes" } },
                };

                if(!string.IsNullOrEmpty(requestParmas.SearchKey))
                {
                    parameters.SearchFields = new List<string>() { "Title", "Description" };
                    parameters.SearchTerm = requestParmas.SearchKey;
                }

                if(requestParmas.Genres.Any())
                {
                    parameters.Filters.Add("Genres", requestParmas.Genres);
                }

                if (requestParmas.DurationMinutes > 0)
                {
                    parameters.Filters.Add("DurationMinutes", requestParmas.DurationMinutes);
                }

                var movies = await _movieService.GetAll(parameters, config);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
