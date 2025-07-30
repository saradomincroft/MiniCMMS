public class UpdateMaintenanceTaskDto
{
    public string? Description { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public string? Priority { get; set; }
    public bool? IsCompleted { get; set; }
    public List<int>? TechnicianIdsToAdd { get; set; }
    public List<int>? TechnicianIdsToRemove { get; set; }

}