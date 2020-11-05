using ApiProject.Models;
using ApiProject.Repositories;
using ApiProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiProject.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _repository;

        /// <summary>
        ///     The params are injected via the startup file
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="config"></param>
        /// <param name="repository"></param>
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config, IUserRepository repository)
        {
            _userManager = userManager;
            _config = config;
            _repository = repository;
        }

        /// <summary>
        ///     Login route, when a user wants to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/v1/users/login")]
        [HttpPost]
        public async Task<IActionResult> GetAllUsers(LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var response = new
                    {
                        message = "Please enter your email and password"
                    };

                    return BadRequest(response);
                }

                var user = new ApplicationUser { Email = model.Email };
                var usr = await _repository.GetUserByEmail(user.Email);

                if (usr == null)
                {
                    // if the email provided is not a valid email address
                    var response = new
                    {
                        message = "The credentials you provided is incorrect"
                    };

                    return BadRequest(response);
                }

                // Checks if password is a match
                var isMatch =
                    (int)_userManager.PasswordHasher.VerifyHashedPassword(usr, usr.PasswordHash, model.Password);

                if (isMatch == 1)
                {
                    // var role = await _userManager.GetRolesAsync(usr);

                    // A jwt token is generated for each user
                    var token = GenerateToken(usr.Id);

                    var response = new
                    {
                        token
                    };

                    return Ok(response);
                }
                else
                {
                    // if password is not a match it returns an error message
                    var response = new
                    {
                        message = "The credentials you provided is incorrect"
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    error = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        ///     Gets a user by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("api/v1/users/{ID}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string ID)
        {
            try
            {
                var usr = await _repository.GetUserById(ID);

                /// Verifies the token for each user
                if (!VerifyToken(HttpContext, ID))
                {
                    var response = new
                    {
                        message = "Token provided is invalid"
                    };

                    return BadRequest(response);
                }

                if (usr == null)
                {
                    var response = new
                    {
                        message = "User not found"
                    };
                    return NotFound(response);
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
            catch (Exception ex)
            {
                var response = new
                {
                    error = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        ///     Gets a user by ID
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("api/v1/users/LoggedIn")]
        [HttpGet]
        public async Task<IActionResult> LoggedInUser()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token).Claims.FirstOrDefault().Value;

            var usr = await _repository.GetUserById(decoded);


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

        /// <summary>
        ///     Adds a new user to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/v1/users/register")]
        [HttpPost]
        public async Task<IActionResult> AddUser(SignupDTO model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email
                };
                // Adds new user to the database
                var result = await _repository.AddUser(user, model.Password);


                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    // Generates a token when a user registered
                    var token = GenerateToken(user.Id);


                    var response = new
                    {
                        token
                    };

                    return Created("api/v1/users", response);
                }
                else
                {
                    var response = new
                    {
                        message = "Error occured while saving to the database",
                        errors = result.Errors
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    error = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        ///     Updates the user based on the
        ///     ID provided
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("api/v1/users/{ID}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(string ID, UpdateDTO model)
        {
            try
            {
                var usr = await _repository.GetUserById(ID);

                if (!VerifyToken(HttpContext, ID))
                {
                    var response = new
                    {
                        message = "Token provided is invalid"
                    };

                    return BadRequest(response);
                }

                if (usr == null)
                {
                    return NoContent();
                }

                /// Upadating the user
                usr.FirstName = string.IsNullOrEmpty(model.FirstName) ? usr.FirstName : model.FirstName;
                usr.LastName = string.IsNullOrEmpty(model.LastName) ? usr.LastName : model.LastName;
                usr.ModifiedAt = DateTime.Now;
                usr.Email = string.IsNullOrEmpty(model.Email) ? usr.Email : model.Email;


                var result = _repository.UpdateUser(usr);

                if (result.Result.Succeeded)
                {
                    var response = new
                    {
                        message = "User has been updated successfully"
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        message = "Failed to update user",
                        errors = result.Result.Errors
                    };
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Deletes user by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("api/v1/users/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string ID)
        {
            var usr = await _repository.GetUserById(ID);


            if (!VerifyToken(HttpContext, ID))
            {
                var response = new
                {
                    message = "Token provided is invalid"
                };

                return BadRequest(response);
            }

            if (usr == null)
            {
                return NoContent();
            }

            var result = await _repository.DeleteUser(usr);

            if (result.Succeeded)
            {
                var response = new
                {
                    message = "User deleted successfully"
                };

                return Ok(response);
            }
            else
            {
                var response = new
                {
                    message = "Failed to delete user",
                    errors = result.Errors
                };

                return BadRequest(response);
            }
        }


        /// <summary>
        ///     Gets the request headers
        ///     and takes out the authorization header
        ///     for verification
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private bool VerifyToken(HttpContext httpContext, string ID)
        {
            var token = httpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token).Claims.FirstOrDefault().Value;

            var user = _userManager.Users.FirstOrDefault(x => x.Id == ID);

            if (user.Id == decoded)
                return true;
            return false;
        }

        /// <summary>
        ///     A method for generating tokens using
        ///     the user ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GenerateToken(string ID)
        {
            var secret = _config["Jwt:SECRET_KEY"];

            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userID", ID)
                }),
                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}