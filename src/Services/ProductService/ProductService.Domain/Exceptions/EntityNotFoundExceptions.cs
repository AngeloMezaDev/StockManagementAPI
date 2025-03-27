namespace TransactionService.Domain.Exceptions;

public class EntityNotFoundExceptions : Exception
{
    public EntityNotFoundExceptions(string entityName, object id)
        : base($"Entity '{entityName}' with ID {id} was not found")
    {
        EntityName = entityName;
        Id = id;
    }

    public string EntityName { get; }

    public object Id { get; }
}
