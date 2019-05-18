using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Swaksoft.Application.Seedwork.Behaviors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swaksoft.Application.Seedwork.Tests.Extensions
{
	public static class MediatorSetupExtensions {
		public static void SetupRequest<TRequest, TResponse>(this Mock<IMediator> mockMediator, IRequestHandler<TRequest, TResponse> interactor)
			where TRequest : IRequest<TResponse>
		{
			mockMediator.SetupRequest(interactor, null);
		}

		public static void SetupRequest<TRequest, TResponse>(this Mock<IMediator> mockMediator, IRequestHandler<TRequest, TResponse> interactor, IValidator<TRequest> validator)
			where TRequest : IRequest<TResponse>
		{
			mockMediator.SetupRequest(async (request, cancellationToken) => await interactor.Handle(request, cancellationToken), validator);
		}

		public static void SetupRequest<TRequest, TResponse>(this Mock<IMediator> mockMediator, Func<TRequest, CancellationToken, Task<TResponse>> next)
			where TRequest : IRequest<TResponse>
		{
			mockMediator.SetupRequest(next, null);
		}
		
		public static void SetupRequest<TRequest, TResponse>(this Mock<IMediator> mockMediator, Func<TRequest, CancellationToken, Task<TResponse>> next, IValidator<TRequest> validator)
			where TRequest : IRequest<TResponse>
		{
			var behavior = GetValidatorBehavior<TRequest, TResponse>(validator);

			mockMediator.Setup(m => m.Send(It.IsAny<TRequest>(), default))
				.Returns<TRequest, CancellationToken>(async (request, cancellationToken) =>
				{
					return await behavior.Handle(request, cancellationToken, async () => await next(request, cancellationToken));
				});
		}

		public static void SetupCommand<TRequest, TResponse>(this Mock<IMediator> mockMediator, IRequestHandler<TRequest, TResponse> interactor)
			where TRequest : ICommand<TResponse>
		{
			mockMediator.SetupCommand(interactor, null);
		}

		public static void SetupCommand<TRequest, TResponse>(this Mock<IMediator> mockMediator, IRequestHandler<TRequest, TResponse> interactor, IValidator<TRequest> validator)
			where TRequest : ICommand<TResponse>
		{
			mockMediator.SetupCommand(async (request, cancellationToken) => await interactor.Handle(request, cancellationToken), validator);
		}

		public static void SetupCommand<TRequest, TResponse>(this Mock<IMediator> mockMediator, Func<TRequest, CancellationToken, Task<TResponse>> next)
			where TRequest : ICommand<TResponse>
		{
			mockMediator.SetupCommand(next, null);
		}
		
		public static void SetupCommand<TRequest, TResponse>(this Mock<IMediator> mockMediator, Func<TRequest, CancellationToken, Task<TResponse>> next, IValidator<TRequest> validator)
			where TRequest : ICommand<TResponse>
		{
			ValidatorBehavior<TRequest, TResponse> validatorBehavior = GetValidatorBehavior<TRequest, TResponse>(validator);

			var behaviorLoggerMock = new Mock<ILogger<LoggingBehavior<TRequest, TResponse>>>();
			var loggerBehavior = new LoggingBehavior<TRequest, TResponse>(behaviorLoggerMock.Object);

			mockMediator.Setup(m => m.Send(It.IsAny<TRequest>(), default))
				.Returns<TRequest, CancellationToken>(async (request, cancellationToken) =>
				{
					return await validatorBehavior.Handle(request, cancellationToken,
						async () => await loggerBehavior.Handle(request, cancellationToken, async () => await next(request, cancellationToken)));
				});
		}

		public static void SetupQuery<TContext, TRequest, TResponse>(this Mock<IMediator> mockMediator, TContext context, IRequestHandler<TRequest, TResponse> interactor, IValidator<TRequest> validator)
			where TRequest : IQuery<TResponse>
			where TContext: DbContext
		{
			ValidatorBehavior<TRequest, TResponse> validatorBehavior = GetValidatorBehavior<TRequest, TResponse>(validator);
						
			var queryBehavior = new QueryBehavior<TContext, TRequest, TResponse>(context);

			mockMediator.Setup(m => m.Send(It.IsAny<TRequest>(), default))
				.Returns<TRequest, CancellationToken>(async (request, cancellationToken) =>
				{
					return await validatorBehavior.Handle(request, cancellationToken,
						async () => await queryBehavior.Handle(request, cancellationToken, async () => await interactor.Handle(request, cancellationToken)));
				});
		}

		private static ValidatorBehavior<TRequest, TResponse> GetValidatorBehavior<TRequest, TResponse>(IValidator<TRequest> validator)
			where TRequest: IRequest<TResponse>
		{
			var validatorFactoryMock = new Mock<IValidatorFactory>();
			validatorFactoryMock.Setup(v => v.GetValidator<TRequest>()).Returns(validator);

			var loggerMock = new Mock<ILogger<ValidatorBehavior<TRequest, TResponse>>>();
			var validatorBehavior = new ValidatorBehavior<TRequest, TResponse>(validatorFactoryMock.Object, loggerMock.Object);
			return validatorBehavior;
		}
	}
}
