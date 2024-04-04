namespace PassIn.Exceptions;

public class RequestConflictException : PassInException
{
    public RequestConflictException(string message)
        : base(message)
    {
        
    }
}
