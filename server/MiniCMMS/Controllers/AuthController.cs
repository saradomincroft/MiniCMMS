using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniCMMS.Data;
using MiniCMMS.Models;
using MiniCMMS.DTOs;
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

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Username = generatedUsername,
            Role = dto.Role,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully",
            username = user.Username
        });
    }
}

// public record RegisterDto(string FirstName, string LastName, string Email, string Username, string Password, string Role);
public record LoginDto(string Username, string Password);