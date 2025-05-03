using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Application.Abstraction;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Shared;
using System.Shared.DTOs;
using System.Text;

namespace System.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IStoreService _storeService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IStoreService storeService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _storeService = storeService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLoginDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest(new ApiResponse<object>("الإيميل وكلمة المرور مطلوبين", 400));
            }

            var storesResponse = await _storeService.GetStoresAsync();
            if (!storesResponse.IsSuccess)
            {
                return StatusCode(storesResponse.StatusCode, storesResponse);
            }

            var store = storesResponse.Data.FirstOrDefault(s => s.OwnerEmail == userDto.Email);
            if (store == null)
            {
                return BadRequest(new ApiResponse<object>("الإيميل غير مربوط بأي محل", 400));
            }

            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<object>("الإيميل مسجل بالفعل", 400));
            }

            var user = new IdentityUser
            {
                UserName = userDto.Email,
                Email = userDto.Email
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiResponse<object>($"فشل التسجيل: {errors}", 400));
            }

           
            await _userManager.AddToRoleAsync(user, "Owner");

            return Ok(new ApiResponse<object>("تم التسجيل بنجاح", 201));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userDto.Password))
            {
                return Unauthorized(new ApiResponse<object>("الإيميل أو كلمة المرور غير صحيحة", 401));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(5000),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}