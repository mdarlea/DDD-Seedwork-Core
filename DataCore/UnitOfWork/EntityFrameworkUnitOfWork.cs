using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCore.Extensions;
using Domain.Events;
using Microsoft.EntityFrameworkCore;
using Swaksoft.Domain.Seedwork;

namespace Swaksoft.Infrastructure.Data.Seedwork.UnitOfWork
{
    public abstract class EntityFrameworkUnitOfWork : DbContext, IUnitOfWork
    {        
		private readonly IDomainMediator domainEventsDispatcher;

		protected EntityFrameworkUnitOfWork(DbContextOptions option, IDomainMediator domainEventsDispatcher) 
			: base(option)
        {   
			this.domainEventsDispatcher = domainEventsDispatcher ?? throw new System.ArgumentNullException(nameof(domainEventsDispatcher));
		}
        

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
		{
			var domainEvents = this.GetDomainEvents();

			var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

			var tasks = domainEvents
				.Select(async (domainEvent) => {
					await domainEventsDispatcher.Publish(domainEvent);
				});

			await Task.WhenAll(tasks);

			return result;
		}

		public void Commit()
		{
			SaveChanges();
		}

		public void SaveChangesAndRefreshChanges()
		{
			bool saveFailed;

			do
			{
				try
				{
					SaveChanges();

					saveFailed = false;

				}
				catch (DbUpdateConcurrencyException ex)
				{
					saveFailed = true;

					ex.Entries.ToList()
							  .ForEach(entry => entry.OriginalValues.SetValues(entry.GetDatabaseValues()));

				}
			} while (saveFailed);
		}

		public void RollbackChanges()
		{
			// set all entities in change tracker 
			// as 'unchanged state'
			ChangeTracker.Entries()
							  .ToList()
							  .ForEach(entry => entry.State = EntityState.Unchanged);
		}

		void IUnitOfWork.SaveChanges()
		{
			base.SaveChanges();
		}

		public async Task<int> SaveChangesAsync()
		{
			return await base.SaveChangesAsync();
		}
	}
}
