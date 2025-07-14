using BookingApi.Data;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingApi.Controllers   
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly byte[] _key;

        public AuthController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            _key = Encoding.UTF8.GetBytes(jwtKey);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User u)
        {
            if (_db.Users.Any(x => x.Email == u.Email))
                return BadRequest("Email already in use");

            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(u.PasswordHash);
            _db.Users.Add(u);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User creds)
        {
            var user = _db.Users.SingleOrDefault(x => x.Email == creds.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(creds.PasswordHash, user.PasswordHash))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { 
                token = tokenHandler.WriteToken(token),
                user = new { 
                    id = user.Id, 
                    email = user.Email 
                }
            });
        }
    }
}