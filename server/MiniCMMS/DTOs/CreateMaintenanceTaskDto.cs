public class CreateMaintenanceTaskDto
{
    public string Description { get; set; } = "";
    public DateTime? ScheduledDate { get; set; }
    public string Priority { get; set; } = "Low";
    public int AssetId { get; set; }
    public int CreatedById { get; set; }
    public List<int>? TechnicianIds { get; set; }
}