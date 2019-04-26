using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork.Behaviors
{
	public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest: ICommand<TResponse>
	{
		private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;
		public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => this.logger = logger;

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			logger.LogInformation("----- Handling command {CommandName} ({@Command})", request.GetType().Name, request);

			var response = await next();

			logger.LogInformation("----- Command {CommandName} handled - response: {@Response}", request.GetType().Name, response);

			return response;
		}
	}
}
