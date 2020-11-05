using ApiProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProject.Controllers
{
    /// <summary>
    ///     Adding functionalities for admin
    /// </summary>
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///     Admin can view users
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/admin/users/{page:int=1}")]
        [HttpGet]
        public IActionResult GetAllUsers(int page)
        {
            if (page < 1) return NotFound();


            var result = _userManager.Users.Skip(5 * (page - 1)).Take(5);


            if (result.Count() == 0) return Ok("Page does not exists");


            var users = new List<UserDataDTO>();


            foreach (var user in result)
                users.Add(
                    new UserDataDTO
                    {
                        LastName = user.LastName,
                        FirstName = user.FirstName,
                        Email = user.Email,
                        Id = user.Id,
                        PasswordHash = user.PasswordHash,

                        CreatedAt = user.CreatedAt,
                        ModifiedAt = user.ModifiedAt
                    });
            return Ok(users);
        }

        /// <summary>
        ///     Admin can get user by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("api/v1/admin/users/{ID}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string ID)
        {
            var usr = await _userManager.FindByIdAsync(ID);


            if (usr == null)
            {
                return NoContent();
            }

            var user = new UserDataDTO
            {
                Email = usr.Email,
                Id = usr.Id,
                PasswordHash = usr.PasswordHash,
                FirstName = usr.FirstName,
                LastName = usr.LastName,
                CreatedAt = usr.CreatedAt,
                ModifiedAt = usr.ModifiedAt
            };


            return Ok(user);
        }
    }
}