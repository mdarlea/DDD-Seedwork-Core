using Swaksoft.Application.Seedwork.Validation;
using Swaksoft.Infrastructure.Crosscutting.Validation;
using System.Linq;

namespace Swaksoft.Application.Seedwork.Extensions
{
	public static class ValidationExtensions
	{
		public static void ValidateEntity<TEntity>(this IEntityValidatorFactory entityValidatorFactory, TEntity entity)
			where TEntity : class
		{
			var entityValidator = entityValidatorFactory.Create();
			var isValid = entityValidator.IsValid(entity);

			if (!isValid) {
				var errors = entityValidator.GetInvalidMessages(entity).ToList();

				throw new ValidationErrorsException(errors);
			}
		}
	}
}
