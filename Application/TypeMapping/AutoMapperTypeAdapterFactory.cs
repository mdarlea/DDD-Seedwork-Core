using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Swaksoft.Infrastructure.Crosscutting.TypeMapping;

namespace Swaksoft.Application.Seedwork.TypeMapping
{
    public class AutoMapperTypeAdapterFactory
        :ITypeAdapterFactory
    {

        /// <summary>
        /// Create a new Automapper type adapter factory
        /// </summary>
        public AutoMapperTypeAdapterFactory(params Assembly[] autoMapperAssemblies)
        {
			if (autoMapperAssemblies == null)
			{
				throw new ArgumentNullException(nameof(autoMapperAssemblies));
			}
			//scan all assemblies finding Automapper Profile
			//ToDo: Fix the Unity issue
			var profiles = autoMapperAssemblies									
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => t.BaseType == typeof(AutoMapperProfile));

            Mapper.Initialize(cfg =>
            {
                foreach (var item in profiles)
                {
                   cfg.AddProfile(Activator.CreateInstance(item) as Profile);
                } 
            });
        }

        public ITypeAdapter Create()
        {
            return new AutoMapperTypeAdapter();
        }
    }
}
