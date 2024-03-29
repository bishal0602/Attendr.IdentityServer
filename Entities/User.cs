﻿using System.ComponentModel.DataAnnotations;

namespace Attendr.IdentityServer.Entities
{
    public class User : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        //[MaxLength(200)]
        //[Required]
        //public string Subject { get; set; }

        [Required]
        [MaxLength(200)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }

        [Required]
        [MaxLength(250)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(200)]
        public string VerificationCode { get; set; }
        public DateTime VerificationCodeExpirationDate { get; set; }


        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();


        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
}
