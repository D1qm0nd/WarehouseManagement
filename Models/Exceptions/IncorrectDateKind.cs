namespace Models.Exceptions;


public class IncorrectDateKindException : Exception
{
    public IncorrectDateKindException(String? message = null) : base(message: message ?? "Incorrect date kind")
    {
    }
}