using Microsoft.AspNetCore.Http;
using Shared.Services.IServices;
using System.Security.Claims;

namespace Shared.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
        }

        public string UserId => _user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string UserName => _user?.FindFirst(ClaimTypes.Name)?.Value;
        public string Email => _user?.FindFirst(ClaimTypes.Email)?.Value;
        public string Role => _user?.FindFirst(ClaimTypes.Role)?.Value;

        public Guid? CinemaId
        {
            get
            {
                var claim = _user?.FindFirst("CinemaId")?.Value;
                return Guid.TryParse(claim, out var guid) ? guid : (Guid?)null;
            }
        }

        public bool IsSuperAdmin => Role == "SuperAdmin";
        public bool IsAdmin => Role == "Admin";
        public bool IsCustomer => Role == "Customer";
        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated == true;

        public T GetClaimValue<T>(string claimType)
        {
            var claimValue = _user?.FindFirst(claimType)?.Value;
            if (string.IsNullOrEmpty(claimValue))
                return default(T);

            try
            {
                return (T)Convert.ChangeType(claimValue, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
