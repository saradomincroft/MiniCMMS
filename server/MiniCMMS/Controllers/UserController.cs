using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MiniCMMS.Data;
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
    [Authorize]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdToString, out int userId))
            return Unauthorized("Invalid user ID claim");

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        return Ok(new
        {
            UserId = user.Id,
            user.Username,
            UserType = user.GetType().Name
        });
    }
}