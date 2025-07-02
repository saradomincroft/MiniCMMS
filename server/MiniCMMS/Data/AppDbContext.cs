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

    // The below is an override method
    // Override's base behaviour of EF Core
    // Automatically runs when EF Core starts and tries to build db model
    // Lets you customise the way tables/ relationships created
    // In my example I'm using the override method to configure
    // many-to-many relationship through custom joint table 'TasksAssignment'
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set composite key (UserId + MaintenanceTaskid)
        modelBuilder.Entity<TasksAssignment>()
            .HasKey(ta => new { ta.UserId, ta.MaintenanceTaskId });

        // Set up foreign key from TasksAssignment to User
        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.User)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(ta => ta.UserId);

        // Set up foreign key from TasksAssignment to MaintenanceTask
        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.MaintenanceTask)
            .WithMany(mt => mt.AssignedUsers)
            .HasForeignKey(ta => ta.MaintenanceTaskId);
    }
}
