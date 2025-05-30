﻿using Microsoft.AspNetCore.Mvc;
using P3.SearchService.Services;

namespace P3.SearchService.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly MovieSearchService _movieSearchService;

        public SearchController(MovieSearchService movieSearchService)
        {
            _movieSearchService = movieSearchService;
        }

        [HttpGet("get-movies")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieSearchService.GetAllMoviesAsync();
            return Ok(movies);
        }
    }
}
