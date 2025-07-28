public class MaintenanceTaskDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public string Priority { get; set; } = "Low";
    public bool IsCompleted { get; set; } = false;
    public DateOnly? CompletedDate { get; set; }
    public int AssetId { get; set; }
    public string AssetName { get; set; } = "";
    public string AssetMainLocation { get; set; } = "";
    public string AssetSubLocation { get; set; } = "";
    public int CreatedById { get; set; }
    public string CreatedByFirstName { get; set; } = "";
    public string CreatedByLastName { get; set; } = "";
    public List<TechnicianDto>? Technicians { get; set; }
}