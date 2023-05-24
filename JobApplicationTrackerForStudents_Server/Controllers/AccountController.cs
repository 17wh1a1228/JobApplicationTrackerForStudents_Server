using JobApplicationTrackerForStudents_Server.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using JobApplicationTrackerForStudents_Server.Models;
using JobApplicationTrackerForStudents_Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JobApplicationTrackerForStudents_Server.Interface;



using JobApplicationTrackerForStudents_Server.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using JobApplicationTrackerForStudents_Server.Interface;

namespace JobApplicationTrackerForStudents_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationsUser> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly JobApplicationsTrackerContext _context;
        private readonly IPasswordHasherWrapper _passwordHasher;


        public AccountController(UserManager<ApplicationsUser> userManager, JwtHandler jwtHandler, JobApplicationsTrackerContext context, IPasswordHasherWrapper passwordHasher)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("Students")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            ApplicationsUser? user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new LoginResult
                {
                    Success = false,
                    Message = "Invalid Username or Password."
                });
            }
            var newStudent = new Student
            {
                Username = loginRequest.UserName,
                PasswordHash = loginRequest.Password
            };

            //_context.Users.Add(newStudent);
            //await _context.SaveChangesAsync();

            JwtSecurityToken secToken = await _jwtHandler.GetTokenAsync(user);
            string? jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new LoginResult
            {
                Success = true,
                Message = "Login successful",
                Token = jwt
            });
        }

        [HttpGet("Students/{id}")]
        [Authorize] 
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            var studentCsv = new StudentCsv
            {
                studentId = student.StudentId,
                username = student.Username, 
                email= student.Email,
                phoneNumber = student.Phone,
                url= student.Url
            };

            return Ok(studentCsv);
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] SignUpRequest signUp)
        {
            var existingUser = await _context.Students.FirstOrDefaultAsync(u => u.Username == signUp.UserName);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            ApplicationsUser user = new ApplicationsUser { UserName= signUp.UserName, Email=signUp.Email };
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, signUp.Password);
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new SignUpResult
                {
                    Success = false,
                    Message = "Registration failed"
                });
            }
            var newUser = new Student
            {
                Username = signUp.UserName,
                PasswordHash = signUp.Password,
                Email = signUp.Email,
                Phone = signUp.Phone,
                Url = signUp.Url
            };

            _context.Students.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User successfully created." });
        }
    }
}