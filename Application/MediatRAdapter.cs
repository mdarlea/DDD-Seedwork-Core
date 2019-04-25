using Domain.Events;
using MediatR;
using Swaksoft.Domain.Seedwork.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork
{
	public class MediatRAdapter : IDomainMediator
	{
		private readonly MediatR.IMediator mediator;

		public MediatRAdapter(MediatR.IMediator mediator)
		{
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task Publish(IDomainEvent notification, CancellationToken cancellationToken = default)
		{
			var type = notification.GetType();
			var adapterType = typeof(NotificationAdapter<>).MakeGenericType(type);
			var adapter = Activator.CreateInstance(adapterType, notification) as INotification;
			await mediator.Publish(adapter);

		}

		public async Task Publish<TNotification>(TNotification notification,	CancellationToken cancellationToken = default)
			where TNotification : IDomainEvent
		{
			var notification2 = new NotificationAdapter<TNotification>(notification);
			await mediator.Publish(notification2, cancellationToken);
		}
	}

}
