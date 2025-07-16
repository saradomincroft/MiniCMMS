namespace MiniCMMS.Dtos;

public class AssetDto
{
    public string Name { get; set; } = "";
    public string MainLocation { get; set; } = "";
    public string SubLocation { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime LastMaintained { get; set; } = DateTime.UtcNow;

}