namespace TransactionService.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object id)
        : base($"Entity '{entityName}' with ID {id} was not found")
    {
        EntityName = entityName;
        Id = id;
    }

    public string EntityName { get; }

    public object Id { get; }
}
