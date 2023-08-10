namespace appointmentSystem.Models;

public class TimeSlot
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Availability { get; set; }
    
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}