using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCore.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swaksoft.Domain.Seedwork;

namespace Swaksoft.Infrastructure.Data.Seedwork.UnitOfWork
{
    public abstract class EntityFrameworkUnitOfWork : DbContext, IUnitOfWork
    {        
		private readonly IMediator domainEventsDispatcher;

		protected EntityFrameworkUnitOfWork(DbContextOptions option, IMediator domainEventsDispatcher) 
			: base(option)
        {   
			this.domainEventsDispatcher = domainEventsDispatcher ?? throw new System.ArgumentNullException(nameof(domainEventsDispatcher));
		}
        

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

			// Dispatch Domain Events collection. 
			// Choices:
			// A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
			// side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
			// B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
			// You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
			await domainEventsDispatcher.DispatchDomainEventsAsync(this);

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
