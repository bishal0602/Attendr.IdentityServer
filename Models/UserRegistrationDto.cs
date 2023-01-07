using System.ComponentModel.DataAnnotations;

namespace Attendr.IdentityServer.Models
{
    public class UserRegistrationDto
    {
        [Required]
        [MaxLength(200)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(200)]
        public string Password { get; set; }

    }
}
