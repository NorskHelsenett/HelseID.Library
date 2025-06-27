namespace HelseId.Library.Exceptions;

public class HelseIdException : Exception
{
    public HelseIdException(TokenErrorResponse errorResponse) : base(errorResponse.ErrorDescription)
    {
        Error = errorResponse.Error;
    }
    
    public string Error { get; init; }
}
