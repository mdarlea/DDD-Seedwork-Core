namespace Swaksoft.Domain.Seedwork.Events
{
  public class DomainEvents : IDomainEvents
	{ 
    private readonly IHandleDomainEvents domainEventsHandler;
      
    public DomainEvents(IHandleDomainEvents domainEventsHandler)
    {
        this.domainEventsHandler = domainEventsHandler ?? throw new System.ArgumentNullException(nameof(domainEventsHandler));        
    }

     //Raises the given domain event
     public void Raise<T>(T args) where T : IDomainEvent
     {
         if (domainEventsHandler != null)
         {
            domainEventsHandler.Handle(args);    
         }
     }
 } 
}
