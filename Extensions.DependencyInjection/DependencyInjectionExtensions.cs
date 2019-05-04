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
using System.Reflection;

namespace Swaksoft.Extensions.DependencyInjection
{
	public static class DependencyInjectionExtensions
	{
		public static void AddBuildingBlocks(this IServiceCollection services, params Assembly[] autoMapperAssemblies) {
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

			services.AddTransient<IDomainMediator, MediatRAdapter>();

			services.AddTransient<IEntityValidatorFactory, DataAnnotationsEntityValidatorFactory>();

			services.AddAutoMapper(autoMapperAssemblies);

			//sigletons
			TypeAdapterLocator.SetCurrent(new AutoMapperTypeAdapterFactory());
			EntityValidatorLocator.SetCurrent(new DataAnnotationsEntityValidatorFactory());
		}
	}
}
