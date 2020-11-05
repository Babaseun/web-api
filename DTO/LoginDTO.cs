using System.ComponentModel.DataAnnotations;

namespace ApiProject.ViewModel
{
    public class LoginDTO
    {
        /// <summary>
        ///     Model to be presented
        ///     when a user decides
        ///     to login
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}