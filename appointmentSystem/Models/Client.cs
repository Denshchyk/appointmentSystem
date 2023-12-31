using System;
using System.Collections.Generic;

namespace appointmentSystem.Models;

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}