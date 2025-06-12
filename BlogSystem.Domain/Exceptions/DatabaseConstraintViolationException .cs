namespace BlogSystem.Domain.Exceptions;

public class DatabaseConstraintViolationException : Exception
{
    public DatabaseConstraintViolationException(string message) : base(message)
    {
        
    }
}
