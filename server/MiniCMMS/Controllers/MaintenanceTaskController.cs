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
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _context.MaintenanceTasks
            .Include(t => t.Asset)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedUsers)
                .ThenInclude(ta => ta.Technician)
            .ToListAsync();

        var result = tasks.Select(t => new MaintenanceTaskDto
        {
            Id = t.Id,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            ScheduledDate = t.ScheduledDate,
            Priority = t.Priority,
            IsCompleted = t.IsCompleted,
            CompletedDate = t.CompletedDate,
            AssetId = t.AssetId,
            AssetName = t.Asset?.Name ?? "",
            AssetMainLocation = t.Asset?.MainLocation ?? "",
            AssetSubLocation = t.Asset?.SubLocation ?? "",
            CreatedById = t.CreatedById,
            CreatedByFirstName = t.CreatedBy?.FirstName ?? "",
            CreatedByLastName = t.CreatedBy?.LastName ?? "",
            Technicians = t.AssignedUsers.Select(ta => new TechnicianDto
            {
                Id = ta.TechnicianId,
                FirstName = ta.Technician?.FirstName ?? "",
                LastName = ta.Technician?.LastName ?? ""
            }).ToList()
        }).ToList();

        return Ok(result);
    }


    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateMaintenanceTask([FromBody] CreateMaintenanceTaskDto dto)
    {
        // Authorisation logic
        // Retrieves and validates current authenticated user ID from their JWT
        // Parses and validates user identity
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Tries to convert string to int
        // If fails (eg claim is missing or NaN) - returns false
        // if parse successful, variable (currentUserId) holds user ID as an int
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

    [Authorize(Roles = "Manager")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMaintenanceTask(int id)
    {
        var maintenanceTask = await _context.MaintenanceTasks
            .Include(t => t.AssignedUsers)
            .Include(t => t.Asset)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (maintenanceTask == null)
        {
            return NotFound("Maintenance ask not found");
        }

        _context.TasksAssignments.RemoveRange(maintenanceTask.AssignedUsers);
        _context.MaintenanceTasks.Remove(maintenanceTask);

        await _context.SaveChangesAsync();

        var message = $"{maintenanceTask.Description} (from {maintenanceTask.Asset?.Name}, {maintenanceTask.Asset?.MainLocation}, {maintenanceTask.Asset?.SubLocation}) has been deleted.";

        return Ok(new { message });
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateMaintenanceTask(int id, UpdateMaintenanceTaskDto dto)
    {
        // Get user ID from claims
        // todo: create helper reusable
        var userIdToString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdToString, out int currentUserId))
            return Unauthorized("Invalid user Id claim");

        // Find task to update
        var maintenanceTask = await _context.MaintenanceTasks
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (maintenanceTask == null)
            return NotFound("Maintenance task not found");

        // get current user
        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null)
            return Unauthorized("User not found");

        if (user is Manager)
        {
            if (dto.Description != null)
                maintenanceTask.Description = dto.Description;
            if (dto.ScheduledDate.HasValue)
                maintenanceTask.ScheduledDate = dto.ScheduledDate.Value;
            if (dto.Priority != null)
                maintenanceTask.Priority = dto.Priority;
            if (dto.IsCompleted.HasValue && dto.IsCompleted.Value != maintenanceTask.IsCompleted)
            {
                maintenanceTask.IsCompleted = dto.IsCompleted.Value;
                maintenanceTask.CompletedDate = dto.IsCompleted.Value ? DateOnly.FromDateTime(DateTime.UtcNow) : null;
            }
            if (dto.TechnicianIdsToAdd != null)
            {
                var currentTechnicianIds = maintenanceTask.AssignedUsers
                    .Select(ta => ta.TechnicianId)
                    .ToList();

                var techniciansToAdd = dto.TechnicianIdsToAdd.Except(currentTechnicianIds);

                foreach (var technicianId in techniciansToAdd)
                {
                    maintenanceTask.AssignedUsers.Add(new TasksAssignment
                    {
                        TechnicianId = technicianId,
                        MaintenanceTaskId = maintenanceTask.Id
                    });
                }
            }

            if (dto.TechnicianIdsToRemove != null)
            {
                foreach (var technicianId in dto.TechnicianIdsToRemove)
                {

                    var taskAssignment = maintenanceTask.AssignedUsers
                        .FirstOrDefault(ta => ta.TechnicianId == technicianId);
                    if (taskAssignment != null)
                    {
                        maintenanceTask.AssignedUsers.Remove(taskAssignment);
                    }
                }
            }
        }

        else if (user is Technician technician)
            {
                // Technicians can only update completion status on tasks assigned to them
                if (dto.IsCompleted.HasValue && dto.IsCompleted.Value != maintenanceTask.IsCompleted)
                {
                    bool isAssigned = maintenanceTask.AssignedUsers.Any(ta => ta.TechnicianId == technician.Id);
                    if (!isAssigned)
                        return Forbid("You are not assigned to this task.");

                    maintenanceTask.IsCompleted = dto.IsCompleted.Value;
                    maintenanceTask.CompletedDate = dto.IsCompleted.Value ? DateOnly.FromDateTime(DateTime.UtcNow) : null;
                }
                else
                {
                    return Forbid("Technicians can only update completion status.");
                }
            }
            else
            {
                return Forbid("User not authorized.");
            }

            await _context.SaveChangesAsync();

            var updatedTask = await _context.MaintenanceTasks
                .Include(t => t.AssignedUsers)
                    .ThenInclude(ta => ta.Technician)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (updatedTask == null)
                return NotFound("Updated maintenance task not found.");

            var response = new MaintenanceTaskDto
            {
                Id = updatedTask.Id,
                Description = updatedTask.Description,
                ScheduledDate = updatedTask.ScheduledDate,
                Priority = updatedTask.Priority,
                IsCompleted = updatedTask.IsCompleted,
                CompletedDate = updatedTask.CompletedDate,
                Technicians = updatedTask.AssignedUsers.Select(ta => new TechnicianDto
                {
                    Id = ta.TechnicianId,
                    FirstName = ta.Technician?.FirstName ?? "",
                    LastName = ta.Technician?.LastName ?? ""
                }).ToList()
            };
            return Ok(response);
        }

}