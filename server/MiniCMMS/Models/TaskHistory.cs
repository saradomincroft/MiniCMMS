using System;

namespace MiniCMMS.Models;

public class TaskHistory
{
    public int Id { get; set; }
    public int MaintenanceTaskId { get; set; }
    public MaintenanceTask? MaintenanceTask { get; set; }
    public DateTime CompletedOn { get; set; }
    public string Notes { get; set; } = "";

}