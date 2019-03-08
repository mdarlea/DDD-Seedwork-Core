using System;
using System.Linq;
using System.Threading.Tasks;
using Swaksoft.Core.Dto;
using Swaksoft.Domain.Seedwork;
using Swaksoft.Domain.Seedwork.Aggregates;
using Swaksoft.Infrastructure.Crosscutting.Extensions;
using Swaksoft.Infrastructure.Crosscutting.Validation;

namespace Swaksoft.Application.Seedwork.Extensions
{
    public class EntityResult<T>
        where T:class 
    {
        private readonly IEntityValidator entityValidator;
        private readonly T entity;
        
        internal EntityResult(IEntityValidator entityValidator, T entity, bool isValid)
        {
            if (entityValidator == null) throw new ArgumentNullException(nameof(entityValidator));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            this.entityValidator = entityValidator;
            this.entity = entity;
            IsValid = isValid;
        }

        public T Entity => entity;
        public bool IsValid { get; }

        public TResult ProjectAs<TResult>()
            where TResult: ActionResult, new()
        {
            if (IsValid) return entity.ProjectedAs<TResult>();
            var errors = entityValidator.GetInvalidMessages(entity).ToList();

            var result = new TResult
            {
                Status = ActionResultCode.Failed,
                Message = "Invalid entity",
                Errors = errors
            };
            return result;
        }
    }
    public static class RepositoryExtensions
    {
        public static async Task<EntityResult<T>> SaveEntityAsync<TEntity, T>(this IRepository<TEntity> repository, T entity)
            where TEntity : class
            where T : class, TEntity
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityValidator = EntityValidatorLocator.CreateValidator();
            var isValid = entityValidator.IsValid(entity);

            if (!isValid) return new EntityResult<T>(entityValidator, entity, false);

			var context = repository.UnitOfWork;

			repository.Add(entity);
            await context.SaveChangesAsync();
            
            return new EntityResult<T>(entityValidator, entity, true);
        }

        public static EntityResult<T> SaveEntity<TEntity,T>(this IRepository<TEntity> repository, T entity)
            where TEntity : class
            where T: class, TEntity
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityValidator = EntityValidatorLocator.CreateValidator();
            var isValid = entityValidator.IsValid(entity);

            if (!isValid) return new EntityResult<T>(entityValidator, entity, false);

			var context = repository.UnitOfWork;
            repository.Add(entity);
            context.SaveChanges();    
            
            return new EntityResult<T>(entityValidator,entity,true);
        }
    }
}
