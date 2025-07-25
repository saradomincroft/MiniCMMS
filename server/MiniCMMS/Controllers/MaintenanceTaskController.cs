using Microsoft.AspNetCore.Mvc;
using MiniCMMS.Data;
using MiniCMMS.Models;
using MiniCMMS.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MiniCMMS.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize]
public class MaintenanceTasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public MaintenanceTasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTasksForUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found");

        if (user is Manager)
        {
            var allTasks = await _context.MaintenanceTasks
                .Include(t => t.Asset)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.Technician)
                .ToListAsync();

            return Ok(allTasks);
        }

        else if (user is Technician technician)
        {
            var technicianTasks = await _context.MaintenanceTasks
                .Include(t => t.Asset)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.Technician)
                .Where(t => t.AssignedUsers.Any(ta => ta.TechnicianId == technician.Id))
                .ToListAsync();

            return Ok(technicianTasks);
        }
        return Forbid("Invalid role");
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateMaintenanceTask([FromBody] CreateMaintenanceTaskDto dto)
    {
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdToString, out int currentUserId))
            return Unauthorized("Invalid user ID claim");

        var maintenanceTask = new MaintenanceTask
        {
            Description = dto.Description,
            ScheduledDate = dto.ScheduledDate,
            Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Low" : dto.Priority,
            AssetId = dto.AssetId,
            CreatedById = currentUserId,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        if (dto.TechnicianIds != null)
        {
            maintenanceTask.AssignedUsers = dto.TechnicianIds
                .Select(id => new TasksAssignment { TechnicianId = id })
                .ToList();
        }

        _context.MaintenanceTasks.Add(maintenanceTask);
        await _context.SaveChangesAsync();

        return Ok(maintenanceTask);
    }

}