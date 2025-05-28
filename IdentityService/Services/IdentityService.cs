using P8.IdentityService.DTOs;
using P8.IdentityService.Models;
using P8.IdentityService.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace P8.IdentityService.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        public IdentityService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<UserDTO> GetCurrentUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if(user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.GenerateToken(user);
                return new UserDTO
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Role = roles[0],
                    Email = user.Email,
                    Token = token
                };
            }
            return null;
        }

        public async Task RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var user = await _userManager.FindByNameAsync(registerRequest.Username);
            if(user != null)
                throw new InvalidOperationException("User already exists.");

            var newUser = new User
            {
                UserName = registerRequest.Username,
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                PasswordHash = registerRequest.Password
            };
            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);
            await _userManager.AddToRoleAsync(newUser, "Customer");

            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                throw new Exception(string.Join(", ", errors));
            }
        }

        public async Task<UserDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.GenerateToken(user);
                return new UserDTO
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Role = roles[0],
                    Email = user.Email,
                    Token = token
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
        }
    }
}
