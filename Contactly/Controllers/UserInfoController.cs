using Contactly.Data;
using Contactly.Models;
using Contactly.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Contactly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly ContactlyDbContext dbContext;
        private readonly IConfiguration configuration;

        public UserInfoController(ContactlyDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = dbContext.UserInfos.ToList();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = dbContext.UserInfos.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(AddUserInfo request)
        {
            var user = dbContext.UserInfos.FirstOrDefault(u =>
                u.UserName == request.UserName && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new[]
{
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
                );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = tokenValue, User = user });

            // User found, login successful
            // Here you would typically generate a token or start a session
            /*return Ok(new { message = "Login successful", userId = user.Id });*/
        }

        [HttpPost("register")]
        public IActionResult Register(AddUserInfo request)
        {
            // Check if the username is already taken
            var existingUser = dbContext.UserInfos.FirstOrDefault(u => u.UserName == request.UserName);
            if (existingUser != null)
            {
                return Conflict("Username is already taken.");
            }

            // Create a new user
            var newUser = new UserInfo
            {
                UserName = request.UserName,
                Password = request.Password,
                // Add other properties as needed
            };

            dbContext.UserInfos.Add(newUser);
            dbContext.SaveChanges();

            // Optionally, you can generate a token or start a session for the new user

            return Ok(new { message = "Registration successful", userId = newUser.Id });
        }
    }
}
