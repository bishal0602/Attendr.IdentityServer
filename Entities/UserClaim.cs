using System.ComponentModel.DataAnnotations;

namespace Attendr.IdentityServer.Entities
{
    public class UserClaim
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(250)]
        [Required]
        public string Type { get; set; }

        [MaxLength(250)]
        [Required]
        public string Value { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public UserClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
