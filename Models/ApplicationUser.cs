using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        ///     Extending Identity by adding
        ///     some fields of my own
        /// </summary>
        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string FirstName { get; set; }

        [Required] [EmailAddress] public override string Email { get; set; }

        public string Photo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}