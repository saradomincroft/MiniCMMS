public class MaintenanceTaskDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime ScheduledDate { get; set; }
    public string Priority { get; set; } = "Low";
    public bool IsCompleted { get; set; } = false;
    public int AssetId { get; set; }

}