using System;

namespace appointmentSystem.Exceptions;

public class BadGatewayException : Exception
{
    public BadGatewayException(string message)
        : base(message)
    { }
}