using Microsoft.Extensions.DependencyInjection;
using Swaksoft.Domain.Seedwork.Events;
using System;
using System.Threading.Tasks;

namespace Domain.Events
{
	public class DomainEventDispatcher : IHandleDomainEvents
	{
		private readonly IServiceProvider serviceProvider;

		public DomainEventDispatcher(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public async Task Handle(IDomainEvent domainEvent)		
		{
			var type = domainEvent.GetType();
			var handleType = typeof(IHandle<>).MakeGenericType(type);
			var theMethod = handleType.GetMethod("Handle");

			var subscribers = serviceProvider.GetServices(handleType);
			foreach (var subscriber in subscribers) {
				try
				{
					
					await (Task) theMethod.Invoke(subscriber, new object[] { domainEvent });
				}
				finally
				{
					var s = subscriber as IDisposable;
					s?.Dispose();
				}
			}			
		}
	}
}
