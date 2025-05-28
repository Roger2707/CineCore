using P8.IdentityService.Models;

namespace P8.IdentityService.Services.IServices
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}
