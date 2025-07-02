using System.Collections.Generic;
namespace MiniCMMS.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "";
    public ICollection<TasksAssignment> AssignedTasks { get; set; } = new List<TasksAssignment>();
}
