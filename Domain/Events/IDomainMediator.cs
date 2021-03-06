﻿using Swaksoft.Domain.Seedwork.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Events
{
	public interface IDomainMediator
	{
		Task Publish(IDomainEvent notification, CancellationToken cancellationToken = default);
		Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
			where TNotification : IDomainEvent;
	}
}
