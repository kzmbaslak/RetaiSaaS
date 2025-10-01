namespace Retail.BuildingBlocks.Domain;


public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly Queue<IDomainEvent> _events = new();
    protected void Raise(IDomainEvent evt) => _events.Enqueue(evt);
    public IEnumerable<IDomainEvent> DequeueDomainEvents()
    {
        while (_events.Count > 0) yield return _events.Dequeue();
    }
}