using AutoMapper;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Swaksoft.Application.Seedwork;
using Swaksoft.Application.Seedwork.Behaviors;
using Swaksoft.Application.Seedwork.TypeMapping;
using Swaksoft.Application.Seedwork.Validation;
using Swaksoft.Infrastructure.Crosscutting.TypeMapping;
using Swaksoft.Infrastructure.Crosscutting.Validation;

namespace Swaksoft.Extensions.DependencyInjection
{
	public static class DependencyInjectionExtensions
	{
		public static void AddBuildingBlocks(this IServiceCollection services) {
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

			services.AddTransient<IDomainMediator, MediatRAdapter>();

			services.AddTransient<IEntityValidatorFactory, DataAnnotationsEntityValidatorFactory>();

			services.AddAutoMapper();

			//sigletons
			TypeAdapterLocator.SetCurrent(new AutoMapperTypeAdapterFactory());
			EntityValidatorLocator.SetCurrent(new DataAnnotationsEntityValidatorFactory());
		}
	}
}
