using appointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace appointmentSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>()
            .HasKey(a => new { a.ClientId, a.TimeSlotId, a.ServiceId });
    }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
}