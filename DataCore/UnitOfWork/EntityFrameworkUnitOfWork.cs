using System.Threading;
using System.Threading.Tasks;
using DataCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Swaksoft.Domain.Seedwork;
using Swaksoft.Domain.Seedwork.Events;

namespace Swaksoft.Infrastructure.Data.Seedwork.UnitOfWork
{
    public abstract class EntityFrameworkUnitOfWork : DbContext, IUnitOfWork
    {
        private readonly IDbContextAdapter _contextAdapter;
		private readonly IHandleDomainEvents domainEventsDispatcher;

		protected EntityFrameworkUnitOfWork(DbContextOptions option, IHandleDomainEvents domainEventsDispatcher) 
			: base(option)
        {            
            _contextAdapter = new DbContextAdapter(this);
			this.domainEventsDispatcher = domainEventsDispatcher ?? throw new System.ArgumentNullException(nameof(domainEventsDispatcher));
		}

        public IDbContextAdapter GetDbContext()
        {
            return _contextAdapter;
        }

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Dispatch Domain Events collection. 
			// Choices:
			// A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
			// side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
			// B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
			// You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
			await domainEventsDispatcher.DispatchDomainEventsAsync(this);

			return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}
	}
}
