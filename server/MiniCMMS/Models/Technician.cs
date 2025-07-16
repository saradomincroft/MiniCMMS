namespace MiniCMMS.Models;

public class Technician : User
{
    public ICollection<TasksAssignment> AssignedTasks { get; set; } = new List<TasksAssignment>();
}