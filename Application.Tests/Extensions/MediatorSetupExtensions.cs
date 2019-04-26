using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Swaksoft.Application.Seedwork.Behaviors;
using System.Threading;

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
			var validatorFactoryMock = new Mock<IValidatorFactory>();
			validatorFactoryMock.Setup(v => v.GetValidator<TRequest>()).Returns(validator);

			var loggerMock = new Mock<ILogger<ValidatorBehavior<TRequest, TResponse>>>();

			var behavior = new ValidatorBehavior<TRequest, TResponse>(validatorFactoryMock.Object, loggerMock.Object);

			mockMediator.Setup(m => m.Send(It.IsAny<TRequest>(), default))
				.Returns<TRequest, CancellationToken>(async (request, cancellationToken) =>
				{
					return await behavior.Handle(request, cancellationToken, async () => await interactor.Handle(request, cancellationToken));
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
			var validatorFactoryMock = new Mock<IValidatorFactory>();
			validatorFactoryMock.Setup(v => v.GetValidator<TRequest>()).Returns(validator);

			var loggerMock = new Mock<ILogger<ValidatorBehavior<TRequest, TResponse>>>();
			var behavior = new ValidatorBehavior<TRequest, TResponse>(validatorFactoryMock.Object, loggerMock.Object);

			var behaviorLoggerMock = new Mock<ILogger<LoggingBehavior<TRequest, TResponse>>>();
			var loggerBehavior = new LoggingBehavior<TRequest, TResponse>(behaviorLoggerMock.Object);
			
			mockMediator.Setup(m => m.Send(It.IsAny<TRequest>(), default))
				.Returns<TRequest, CancellationToken>(async (request, cancellationToken) =>
				{
					return await behavior.Handle(request, cancellationToken, 
						async () => await loggerBehavior.Handle(request, cancellationToken, async () => await interactor.Handle(request, cancellationToken)));
				});
		}
	}
}
