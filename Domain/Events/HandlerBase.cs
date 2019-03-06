using System;
using System.Threading.Tasks;

namespace Swaksoft.Domain.Seedwork.Events
{
    public abstract class HandlerBase<T> : IHandle<T>
        where T:IDomainEvent
    {
        public abstract Task Handle(T args);

        #region dispose
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion dispose
    }
}
