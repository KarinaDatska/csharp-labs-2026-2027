using DroneServiceCenter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Drone> Drones => Set<Drone>();
        public DbSet<Battery> Batteries => Set<Battery>();
        public DbSet<FlightLog> FlightLogs => Set<FlightLog>();
        public DbSet<RepairOrder> RepairOrders => Set<RepairOrder>();
        public DbSet<SerialComponentLog> SerialComponentLogs => Set<SerialComponentLog>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Drone>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<Battery>()
                .HasIndex(b => b.InventoryCode)
                .IsUnique();

            modelBuilder.Entity<FlightLog>()
                .HasOne(f => f.Drone)
                .WithMany(d => d.FlightLogs)
                .HasForeignKey(f => f.DroneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Battery>()
                .HasOne(b => b.Drone)
                .WithMany(d => d.Batteries)
                .HasForeignKey(b => b.DroneId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RepairOrder>()
                .HasOne(r => r.Drone)
                .WithMany(d => d.RepairOrders)
                .HasForeignKey(r => r.DroneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SerialComponentLog>()
                .HasOne(s => s.Drone)
                .WithMany(d => d.SerialComponentLogs)
                .HasForeignKey(s => s.DroneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}