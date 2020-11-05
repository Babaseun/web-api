using System.ComponentModel.DataAnnotations;

namespace ApiProject.ViewModel
{
    public class UpdateDTO
    {
        /// <summary>
        ///     A model for updating user records
        /// </summary>
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string LastName { get; set; }

        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string FirstName { get; set; }

        [EmailAddress] public string Email { get; set; }
    }
}