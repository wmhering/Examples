using Microsoft.EntityFrameworkCore;

namespace EFCoreSpecificationExample.DAL.DbModel;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkItem>()
            .ToTable("WorkItems")
            .HasKey(e => e.WorkItemKey);
        modelBuilder.Entity<WorkItem>()
            .HasMany(e => e.StateChanges)
            .WithOne(e => e.WorkItem)
                .HasForeignKey(e => e.WorkItemKey);

        modelBuilder.Entity<State>()
            .ToTable("States")
            .HasKey(e => e.StateKey);
        modelBuilder.Entity<State>()
            .HasMany(e => e.StateChanges)
            .WithOne(e => e.State)
                .HasForeignKey(e => e.StateKey);

        modelBuilder.Entity<StateChange>()
            .ToTable("StateChanges")
            .HasKey(e => e.StateChangeKey);
            
    }
}
