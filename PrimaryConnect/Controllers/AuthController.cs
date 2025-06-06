﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrimaryConnect.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public AuthController(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // Admin Login
        [HttpPost("login/admin")]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            var admin = await _dbContext.Administrators.SingleOrDefaultAsync(a => a.Email == email);

            if (admin != null && admin.Password == password)
            {
                // ✅ Store admin ID in session
                HttpContext.Session.SetString("UserId", admin.Id.ToString());
                HttpContext.Session.SetString("Role", "Admin");
                // ✅ Generate JWT token
                var token = GenerateJwtToken(admin.Id.ToString(), "Admin");

                return Ok(new
                {
                    role = "Admin",
                    token,
                    admin
                    //admin.Id,
                    //admin.Email,
                    //admin.PhoneNumber,
                    //admin.Password,
                    //admin.Permitions,
                    //admin.SchoolId
                });
            }

            return Unauthorized("Invalid admin credentials");
        }

        //[HttpPost("login/admin")]
        //public async Task<IActionResult> AdminLogin(string email, string password)
        //{
        //    var admin = await _dbContext.Administrators.SingleOrDefaultAsync(a => a.Email == email);

        //    if (admin != null && admin.Password == password)
        //    {
        //        var token = GenerateJwtToken(admin.Id.ToString(), "Admin");
        //        return Ok(new { role = "Admin", token });
        //    }

        //    return Unauthorized("Invalid admin credentials");
        //}

        // Teacher Login

        [HttpPost("login/Parent")]
        public async Task<IActionResult> ParentLogin(string email, string password)
        {
            var admin = await _dbContext.Parents.SingleOrDefaultAsync(a => a.Email == email);

            if (admin != null && admin.Password == password)
            {
                // ✅ Store admin ID in session
                HttpContext.Session.SetString("UserId", admin.Id.ToString());
                HttpContext.Session.SetString("Role","Parent");
                // ✅ Generate JWT token
                var token = GenerateJwtToken(admin.Id.ToString(), "Parent");

                return Ok(new
                {
                    role = "Parent",
                    token,
                    admin
                 
                });
            }

            return Unauthorized("Invalid Parent credentials");
        }


        [HttpPost("login/Teacher")]
        public async Task<IActionResult> TeacherLogin(string email, string password)
        {
            var admin = await _dbContext.Teachers.SingleOrDefaultAsync(a => a.Email == email);

            if (admin != null && admin.Password == password)
            {
                // ✅ Store admin ID in session
                HttpContext.Session.SetString("UserId", admin.Id.ToString());
                HttpContext.Session.SetString("Role", "Teacher");

                // ✅ Generate JWT token
                var token = GenerateJwtToken(admin.Id.ToString(), "Teacher");

                return Ok(new
                {
                    role = "Teacher",
                    token,
                    admin

                });
            }

            return Unauthorized("Invalid Teacher credentials");
        }

        //[HttpPost("login/teacher")]
        //public async Task<IActionResult> TeacherLogin(string email, string password)
        //{
        //    var teacher = await _dbContext.Teachers.SingleOrDefaultAsync(t => t.Email == email);
        //    if (teacher != null && teacher.Password == password)
        //    {
        //        var token = GenerateJwtToken(teacher.Id.ToString(), "Teacher");
        //        return Ok(new { role = "Teacher", token });
        //    }

        //    return Unauthorized("Invalid teacher credentials");
        //}

        // Parent Login
        //[HttpPost("login/parent")]
        //public async Task<IActionResult> ParentLogin(string email, string password)
        //{
        //    var parent = await _dbContext.Parents.SingleOrDefaultAsync(p => p.Email == email);
        //    if (parent != null && parent.Password == password)
        //    {
        //        var token = GenerateJwtToken(parent.Id.ToString(), "Parent");
        //        return Ok(new { role = "Parent", token });
        //    }

        //    return Unauthorized("Invalid parent credentials");
        //}

        // Token generator method
        private string GenerateJwtToken(string userId, string role)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
