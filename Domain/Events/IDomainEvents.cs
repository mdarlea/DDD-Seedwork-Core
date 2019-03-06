namespace Swaksoft.Domain.Seedwork.Events
{
	public interface IDomainEvents
	{
		void Raise<T>(T args) where T : IDomainEvent;
	}
}