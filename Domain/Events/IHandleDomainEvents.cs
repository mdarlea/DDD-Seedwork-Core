using System;
using System.Threading.Tasks;

namespace Swaksoft.Domain.Seedwork.Events
{
    public interface IHandleDomainEvents
    {
        Task Handle(IDomainEvent domainEvent);
    }
}
