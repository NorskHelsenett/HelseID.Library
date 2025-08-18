namespace HelseId.Library.Exceptions;

public class HelseIdException : Exception
{
    public HelseIdException(TokenErrorResponse errorResponse) : base(errorResponse.ErrorDescription)
    {
        Error = errorResponse.Error;
    }
    
    public HelseIdException(string message, string error) : base(message)
    {
        Error = error;
    }
    
    public string Error { get; init; }
}
