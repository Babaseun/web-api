using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var result = new
            {
                message = "Welcome to ASP .NET CORE WEB API 2020",
                LoginEndpoint = "api/v1/users/login",
                RegisterEndpoint = "api/v1/users/register",
                GetUserEndPoint = "api/v1/users/{ID}"
            };

            return Ok(result);
        }
    }
}