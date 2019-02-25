
using System;

namespace Swaksoft.Domain.Seedwork
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextAdapter BeginTransaction();
    }
}
