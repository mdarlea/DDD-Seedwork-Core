using Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Swaksoft.Domain.Seedwork.Events;

namespace Domain.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDomainEvents(this IServiceCollection services) {
			services.AddSingleton<IDomainEvents, DomainEvents>();
			services.AddTransient<IHandleDomainEvents, DomainEventDispatcher>();
		}
	}
}
