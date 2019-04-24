using Swaksoft.Domain.Seedwork.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork
{
	public abstract class NotificationHandler<T> : MediatR.INotificationHandler<NotificationAdapter<T>>
		where T : IDomainEvent
	{
		public async Task Handle(NotificationAdapter<T> notification, CancellationToken cancellationToken)
		{
			await Handle(notification.Notification, cancellationToken);
		}

		protected abstract Task Handle(T notification, CancellationToken cancellationToken);
	}
}
