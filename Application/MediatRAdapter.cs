using Domain.Events;
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

		public Task Publish<TNotification>(TNotification notification,
			CancellationToken cancellationToken = default)
			where TNotification : IDomainEvent
		{
			var notification2 = new NotificationAdapter<TNotification>(notification);
			return mediator.Publish(notification2, cancellationToken);
		}
	}

}
