using IdentityService.DTOs;

namespace IdentityService.Services.IServices
{
    public interface IIdentityService
    {
        Task<UserDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task RegisterAsync(RegisterRequestDTO registerRequest);
        Task<UserDTO> GetCurrentUserAsync(string username);
    }
}
