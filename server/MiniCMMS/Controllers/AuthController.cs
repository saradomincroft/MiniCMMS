using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniCMMS.Data;
using MiniCMMS.Models;
using MiniCMMS.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniCMMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _passwordHasher = new PasswordHasher<User>();
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var capitalisedFirstName = RegisterDto.CapitalisedName(dto.FirstName);
        var capitalisedLastName = RegisterDto.CapitalisedName(dto.LastName);

        var baseUsername = Regex.Replace($"{capitalisedFirstName}{capitalisedLastName}", @"[\s\-]", "").ToLower();
        var generatedUsername = baseUsername;
        int counter = 1;

        while (_context.Users.Any(u => u.Username == generatedUsername))
        {
            counter++;
            generatedUsername = $"{baseUsername}{counter}";
        }

        // checks to see if user already exists email
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Error signing up");

        User user = dto.Role switch
        {
            "Manager" => new Manager
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = generatedUsername,
            },
            "Technician" => new Technician
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = generatedUsername,
            },
            _ => throw new ArgumentException("Invalid role.")
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new Dictionary<string, string> { { "message", "User registered successfully." } });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        // first check if user is logging in with their email or generated username
        bool isEmail = Regex.IsMatch(dto.Identifier, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        // Verify username
        var user = isEmail
            ? _context.Users.FirstOrDefault(u => u.Email == dto.Identifier)
            : _context.Users.FirstOrDefault(u => u.Username == dto.Identifier);

        if (user == null)

            return Unauthorized("Invalid credentials");

        // Verify password
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid credentials");

        string role = user switch
        {
            Manager => "Manager",
            Technician => "Technician",
            _ => "User"
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, role)
        };

        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrEmpty(jwtSecret))
            throw new Exception("JWT_SECRET_KEY is not set in the environment.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
            
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            username = user.Username,
            role = role
        });
        
    }
}

// public record RegisterDto(string FirstName, string LastName, string Email, string Username, string Password, string Role);
public record LoginDto(string Identifier, string Password);