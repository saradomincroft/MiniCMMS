namespace MiniCMMS.Models;

public class TasksAssignment
{
    public int TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public int MaintenanceTaskId { get; set; }
    public MaintenanceTask? MaintenanceTask { get; set; }
}