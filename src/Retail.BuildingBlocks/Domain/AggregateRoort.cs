namespace Retail.BuildingBlocks.Domain;


public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _events = new();
    protected void Raise(IDomainEvent evt) => _events.Add(evt);

    IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _events.AsReadOnly();
    public void ClearDomainEvents() => _events.Clear();
}