using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
