using jwtproject.Model;
using jwtproject.Model.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwtproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new ApplicationUser { 
                UserName = model.UserName,
                Email = model.Email,
                Phone = model.Phone
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authclaim = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                authclaim.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authclaim,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256
                    )

                    );
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role Added Successfully" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Role alrdy Exists");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if(user == null)
            {
                return BadRequest("User not found");
            }

            var result = await _userManager.AddToRoleAsync(user,model.Role);

            if (result.Succeeded)
            {
                return Ok(new { message = "Role Assingend successfully" });
            }
            return BadRequest(result.Errors);
        }
        // دریافت اطلاعات کاربر با شناسه (UserId)
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber
            });
        }

        // دریافت اطلاعات کاربر بر اساس نام کاربری
        [HttpGet("GetUserByUsername/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber
            });
        }

        // دریافت اطلاعات کاربر جاری (با استفاده از توکن JWT)
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber
            });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            if (users == null || users.Count == 0)
            {
                return NotFound(new { message = "No users found" });
            }

            var userList = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber
            });

            return Ok(userList);
        }

        //[Authorize(Roles = "Admin")] // فقط ادمین اجازه ویرایش دارد
        [HttpPost("EditUser/{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] EditUserDto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // بروزرسانی اطلاعات کاربر
                user.UserName = model.UserName ?? user.UserName;
                user.Email = model.Email ?? user.Email;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "User updated successfully", user });
            }
            return BadRequest("Invalid Method.");
        }

        //[Authorize(Roles = "Admin")] // فقط ادمین اجازه حذف دارد
        [HttpPost("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "DELETE")
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "User deleted successfully" });
            }
            return BadRequest("invalid method.");
        }

        [Authorize] // فقط کاربران احراز هویت شده می‌توانند خروج کنند
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully" });
        }


    }
}
