public class UpdateTaskDto
{
    public string? Description { get; set; }
    public DateOnly? ScheduledDate { get; set; }  
    public string? Priority { get; set; }
    public bool? IsCompleted { get; set; }  
}