using System.Linq;
using Swaksoft.Domain.Seedwork;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Swaksoft.Infrastructure.Data.Seedwork.UnitOfWork
{
    internal class DbContextAdapter : IDbContextAdapter
    {
        private readonly DbContext context;

        public DbContextAdapter(DbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            this.context = context;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed;

            do
            {
                try
                {
                    context.SaveChanges();

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
            context.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry => entry.State = EntityState.Unchanged);
        }

        #region IDisposable Members

        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
			if (!disposing) return;

			bool changesMade = context.ChangeTracker.HasChanges();
			if (changesMade) {
				Commit();
			}
		}
        #endregion
    }
}
