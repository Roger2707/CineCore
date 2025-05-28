using Microsoft.AspNetCore.Identity;

namespace P8.IdentityService.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
