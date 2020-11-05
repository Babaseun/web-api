using ApiProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ApiProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///     Gets user by ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserById(string Id)
        {
            var usr = await _userManager.FindByIdAsync(Id);

            return usr;
        }

        /// <summary>
        ///     Gets user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var usr = await _userManager.FindByEmailAsync(email);


            return usr;
        }

        /// <summary>
        ///     Adds user to the database
        ///     and it creates a hash password in the
        ///     process
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityResult> AddUser(ApplicationUser user, string password)
        {
            var res = await _userManager.CreateAsync(user, password);

            return res;
        }

        /// <summary>
        ///     Updates the user based on
        ///     the information provided
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateUser(ApplicationUser user)
        {
            var res = await _userManager.UpdateAsync(user);
            return res;
        }

        /// <summary>
        ///     Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteUser(ApplicationUser user)
        {
            var res = await _userManager.DeleteAsync(user);

            return res;
        }
    }
}