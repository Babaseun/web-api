using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public class SignupDTO
    {
        /// <summary>
        ///     Signup model to be presented when
        ///     a user decides to create an account
        /// </summary>
        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string FirstName { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}