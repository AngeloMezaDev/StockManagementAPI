namespace TransactionService.Domain.Exceptions;
public class DatabaseExceptions : Exception
{
    public DatabaseExceptions(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

