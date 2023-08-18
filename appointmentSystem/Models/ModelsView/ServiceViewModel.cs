namespace appointmentSystem.Models.Models;

public record ServiceViewModel(Guid Id, string Name, TimeSpan Duration, decimal Price);