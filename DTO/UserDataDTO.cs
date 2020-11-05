using System;

namespace ApiProject.Models
{
    public class UserDataDTO
    {
        public string Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}