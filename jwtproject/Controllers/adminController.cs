using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtproject.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class adminController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("you have accessed the admin controller.");
        }
    }
}
