using System;

namespace appointmentSystem.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid TimeSlotId { get; set; }
    
    public virtual Client? Client { get; set; }
    public virtual TimeSlot? TimeSlot { get; set; }
}