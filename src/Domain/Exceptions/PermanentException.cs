using System;
namespace Domain.Exceptions;

public class PermanentException : Exception
{
    public PermanentException(string message): base(message)
    {
    }
}