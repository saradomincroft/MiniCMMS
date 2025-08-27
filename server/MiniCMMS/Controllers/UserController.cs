using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MiniCMMS.Data;
using MiniCMMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MiniCMMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdToString, out int userId))
            return Unauthorized("Invalid user ID claim");

        var user = await _context.Users.FindAsync(userId);

        if (user == null) return NotFound(new { Message = "User not found." });

        return Ok(new
        {
            UserId = user.Id,
            user.Username,
            UserType = user.GetType().Name
        });
    }

    // GET: api/User/technicians
    [HttpGet("technicians")]
    public async Task<IActionResult> GetAllTechnicians()
    {
        // Get current user ID from claims
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdToString, out int userId))
            return Unauthorized("Invalid user ID claim");

        var currentUser = await _context.Users.FindAsync(userId);
        if (currentUser == null) return Unauthorized("User not found");

        // Only allow managers
        if (currentUser is not Manager)
            return Forbid("Only managers can access all technicians");

        var technicians = await _context.Users
            .OfType<Technician>()
            .Select(t => new
            {
                t.Id,
                t.FirstName,
                t.LastName,
                t.Username
            })
            .ToListAsync();

        return Ok(technicians);
    }
}
