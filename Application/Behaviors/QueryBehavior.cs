using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork.Behaviors
{
	public class QueryBehavior<TContext, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IQuery<TResponse>
		where TContext: DbContext
	{
		private readonly TContext context;

		public QueryBehavior(TContext context) {
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var currentTracking = context.ChangeTracker.QueryTrackingBehavior;
			context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

			var response = await next();

			context.ChangeTracker.QueryTrackingBehavior = currentTracking;

			return response;
		}
	}
}
