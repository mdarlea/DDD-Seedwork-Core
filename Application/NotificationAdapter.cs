using Swaksoft.Domain.Seedwork.Events;

namespace Swaksoft.Application.Seedwork
{
	public class NotificationAdapter<T> : MediatR.INotification
		where T: IDomainEvent
	{
		public T Notification { get; }

		public NotificationAdapter(T notification)
		{
			Notification = notification;
		}
	}
}
