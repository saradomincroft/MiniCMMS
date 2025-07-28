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
            IsCompleted = false,
            AssignedUsers = dto.TechnicianIds?.Select(id => new TasksAssignment { TechnicianId = id }).ToList() ?? new()
        };

        _context.MaintenanceTasks.Add(maintenanceTask);
        await _context.SaveChangesAsync();

        var taskWithTechnicians = await _context.MaintenanceTasks
        .AsNoTracking()
        .Include(t => t.Asset)
        .Include(t => t.CreatedBy)
        .Include(t => t.AssignedUsers)
            .ThenInclude(ta => ta.Technician)
        .FirstOrDefaultAsync(t => t.Id == maintenanceTask.Id);

    if (taskWithTechnicians == null)
    return NotFound("Created task not found");

        var resultDto = new MaintenanceTaskDto
        {
            Id = taskWithTechnicians.Id,
            Description = taskWithTechnicians.Description,
            CreatedAt = taskWithTechnicians.CreatedAt,
            ScheduledDate = taskWithTechnicians.ScheduledDate,
            Priority = taskWithTechnicians.Priority,
            IsCompleted = taskWithTechnicians.IsCompleted,
            CompletedDate = taskWithTechnicians.CompletedDate,
            AssetId = taskWithTechnicians.AssetId,
            AssetName = taskWithTechnicians.Asset?.Name ?? "",
            AssetMainLocation = taskWithTechnicians.Asset?.MainLocation ?? "",
            AssetSubLocation = taskWithTechnicians.Asset?.SubLocation ?? "",
            CreatedById = taskWithTechnicians.CreatedById,
            CreatedByFirstName = taskWithTechnicians.CreatedBy?.FirstName ?? "",
            CreatedByLastName = taskWithTechnicians.CreatedBy?.LastName ?? "",

            Technicians = taskWithTechnicians.AssignedUsers != null && taskWithTechnicians.AssignedUsers.Any()
                ? taskWithTechnicians.AssignedUsers
                    .Where(ta => ta.Technician != null)
                    .Select(ta => new TechnicianDto
                    {
                        Id = ta.TechnicianId,
                        FirstName = ta.Technician!.FirstName,
                        LastName = ta.Technician!.LastName
                    }).ToList()
                : new List<TechnicianDto>()
        };

        return Ok(resultDto);
    }

}