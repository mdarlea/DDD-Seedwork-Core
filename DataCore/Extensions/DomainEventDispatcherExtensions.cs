using Microsoft.EntityFrameworkCore;
using Swaksoft.Domain.Seedwork.Aggregates;
using Swaksoft.Domain.Seedwork.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCore.Extensions
{
	public static class DomainEventDispatcherExtensions
	{
		public static async Task DispatchDomainEventsAsync(this IHandleDomainEvents dispatcher, DbContext ctx) {
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			if (ctx == null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			var domainEntities = ctx.ChangeTracker
				.Entries<Entity>()
				.Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

			var domainEvents = domainEntities
				.SelectMany(x => x.Entity.DomainEvents)
				.ToList();

			domainEntities.ToList()
				.ForEach(entity => entity.Entity.ClearDomainEvents());

			var tasks = domainEvents
				.Select(async (domainEvent) => {
					await dispatcher.Handle(domainEvent);
				});

			await Task.WhenAll(tasks);
		}
	}
}
