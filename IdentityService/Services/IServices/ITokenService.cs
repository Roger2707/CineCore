using IdentityService.Models;

namespace IdentityService.Services.IServices
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}
