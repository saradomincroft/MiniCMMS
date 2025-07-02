namespace MiniCMMS.Models;

public class TasksAssignment
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int MaintenanceTaskId { get; set; }
    public MaintenanceTask? MaintenanceTask { get; set; }
}