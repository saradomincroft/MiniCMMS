using System;
using System.Collections.Generic;

namespace MiniCMMS.Models;

public class MaintenanceTask
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime ScheduledDate { get; set; }
    public string Priority { get; set; } = "Low";
    public bool IsCompleted { get; set; } = false;

    public int AssetId { get; set; }
    public Asset? Asset { get; set; }

    public ICollection<TasksAssignment> AssignedUsers { get; set; } = new List<TasksAssignment>();
    public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}