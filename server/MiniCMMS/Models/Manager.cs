namespace MiniCMMS.Models;

public class Manager : User
{
    public ICollection<MaintenanceTask> TasksCreated { get; set; } = new List<MaintenanceTask>();
}