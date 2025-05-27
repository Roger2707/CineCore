using IdentityService.DTOs;
using IdentityService.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var userDTO = await _identityService.LoginAsync(loginRequestDTO);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                await _identityService.RegisterAsync(registerRequestDTO);
                return Ok(new { message = "Registered successfully 👏 Now you can login into app" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            try
            {
                var userDTO = await _identityService.GetCurrentUserAsync(username);
                if (userDTO == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
