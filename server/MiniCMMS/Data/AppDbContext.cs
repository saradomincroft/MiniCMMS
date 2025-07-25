using Microsoft.EntityFrameworkCore;
using MiniCMMS.Models;

namespace MiniCMMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
    public virtual DbSet<Asset> Assets { get; set; }
    public virtual DbSet<TasksAssignment> TasksAssignments { get; set; }

    // The below is an override method
    // Override's base behaviour of EF Core
    // Automatically runs when EF Core starts and tries to build db model
    // Lets you customise the way tables/ relationships created
    // In my example I'm using the override method to configure
    // many-to-many relationship through custom joint table 'TasksAssignment'

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<User>("User")
            .HasValue<Manager>("Manager")
            .HasValue<Technician>("Technician");

        // Set composite key (UserId + MaintenanceTaskid)
        modelBuilder.Entity<TasksAssignment>()
            .HasKey(ta => new { ta.TechnicianId, ta.MaintenanceTaskId });

        // Set up foreign key from TasksAssignment to User
        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.Technician)
            .WithMany(t => t.AssignedTasks)
            .HasForeignKey(ta => ta.TechnicianId);

        // Set up foreign key from TasksAssignment to MaintenanceTask
        modelBuilder.Entity<TasksAssignment>()
            .HasOne(ta => ta.MaintenanceTask)
            .WithMany(mt => mt.AssignedUsers)
            .HasForeignKey(ta => ta.MaintenanceTaskId);

        modelBuilder.Entity<MaintenanceTask>()
            .HasOne(mt => mt.CreatedBy)
            .WithMany(m => m.TasksCreated)
            .HasForeignKey(mt => mt.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
