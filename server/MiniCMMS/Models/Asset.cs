using System;
using System.Collections.Generic;

namespace MiniCMMS.Models;

public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string MainLocation { get; set; } = "";
    public string SubLocation { get; set; } = "";
    public string Category { get; set; } = "";
    public DateOnly LastMaintained { get; set; }
    public ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();

}