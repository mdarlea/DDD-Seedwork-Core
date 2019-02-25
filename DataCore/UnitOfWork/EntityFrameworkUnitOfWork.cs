using Microsoft.EntityFrameworkCore;
using Swaksoft.Domain.Seedwork;

namespace Swaksoft.Infrastructure.Data.Seedwork.UnitOfWork
{
    public abstract class EntityFrameworkUnitOfWork : DbContext, IUnitOfWork
    {
        private readonly IDbContextAdapter _contextAdapter;

        protected EntityFrameworkUnitOfWork(DbContextOptions option) 
			: base(option)
        {            
            _contextAdapter = new DbContextAdapter(this);
        }

        public IDbContextAdapter BeginTransaction()
        {
            return _contextAdapter;
        }        
    }
}
