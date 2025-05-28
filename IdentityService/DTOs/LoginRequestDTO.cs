using System.ComponentModel.DataAnnotations;

namespace P8.IdentityService.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
