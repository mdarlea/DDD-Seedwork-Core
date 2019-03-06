
using System;
using System.Threading.Tasks;

namespace Swaksoft.Domain.Seedwork.Events
{
    public interface IHandle : IDisposable
    {
    }

    public interface IHandle<T> :IHandle where T : IDomainEvent
    {
        Task Handle(T args);
    }
}
