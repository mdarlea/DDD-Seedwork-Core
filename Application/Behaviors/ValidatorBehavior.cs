using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork.Behaviors
{
	public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		private readonly IValidator<TRequest> validator;
		private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> logger;

		public ValidatorBehavior(IValidatorFactory validatorFactory, ILogger<ValidatorBehavior<TRequest, TResponse>> logger) {
			if (validatorFactory == null)
			{
				throw new ArgumentNullException(nameof(validatorFactory));
			}

			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

			validator = validatorFactory.GetValidator<TRequest>();			
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			if (validator != null)
			{
				var failures = validator.Validate(request).Errors
						.Where(error => error != null)
						.ToList();
				if (failures.Any())
				{
					var typeName = request.GetType().Name;

					logger.LogWarning("Validation errors - {RequestType} - Request: {@Request} - Errors: {@ValidationErrors}", typeName, request, failures);
					throw new ValidationException("Validation exception", failures);
				}
			}

			return await next();
		}
	}
}
