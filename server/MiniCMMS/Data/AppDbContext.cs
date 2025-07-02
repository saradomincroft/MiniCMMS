using Microsoft.EntityFrameworkCore;
using MiniCMMS.Models;

namespace MiniCMMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }
    public DbSet<TasksAssignment> TasksAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TasksAssignment>()
            .HasKey(ta => new { ta.UserId, ta.MaintenanceTaskId });

        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.User)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(ta => ta.UserId);

        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.MaintenanceTask)
            .WithMany(mt => mt.AssignedUsers)
            .HasForeignKey(ta => ta.MaintenanceTaskId);
    }
}
