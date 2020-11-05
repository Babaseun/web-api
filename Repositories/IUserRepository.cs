using ApiProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ApiProject.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        ///     An interface for the user repository
        /// </summary>
        Task<IdentityResult> AddUser(ApplicationUser user, string password);

        Task<IdentityResult> DeleteUser(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> UpdateUser(ApplicationUser user);
        Task<ApplicationUser> GetUserById(string Id);
    }
}