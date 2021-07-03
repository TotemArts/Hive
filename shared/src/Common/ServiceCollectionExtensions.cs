using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hive.Shared.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorker<TInterface, TClass>(this IServiceCollection services)
            where TClass : class, TInterface, IHostedService
            where TInterface : class
        {
            _ = services.AddSingleton<TClass>();
            _ = services.AddHostedService(x => x.GetRequiredService<TClass>());
            _ = services.AddSingleton<TInterface>(x => x.GetRequiredService<TClass>());
        }

        /// <summary>
        /// Adds Validators to the <paramref name="services"/> and registers any <see cref="AbstractValidator{T}"/>s found in the main- and referenced assemblies.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to extend.</param>
        /// <param name="namespacePrefixes">Prefixes of namespaces from which to load validators. if none are given, "Hive" is used</param>
        /// <returns><see cref="IValidator{T}"/> with the AutoMapper and <see cref="AbstractValidator{T}"/> registrations.</returns>
        public static IServiceCollection AddFluentValidations(this IServiceCollection services, params string[]? namespacePrefixes)
        {
            if (namespacePrefixes == null || namespacePrefixes.Length == 0)
                namespacePrefixes = new[] { "Hive" };

            ValidatorOptions.Global.DisplayNameResolver = (type, info, arg3) => info.Name; // just return the name of the property

            // The GetEntryAssembly method can return null when a managed assembly has been loaded from an unmanaged application.
            var mainAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

            var allAssemblies = mainAssembly.GetReferencedAssemblies()
                .Union(new[] { mainAssembly.GetName() });

            var validatorTypes = allAssemblies
                .Where(a => namespacePrefixes.Any(x => (a.Name ?? "").StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(an => Assembly.Load(an).GetTypes())
                .Where(p => p.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValidator<>))
                            && p.IsPublic && !p.IsAbstract)
                .Distinct();

            foreach (var validatorType in validatorTypes)
            {
                var interfaceType = validatorType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValidator<>));

                services.Add(new ServiceDescriptor(interfaceType!, validatorType, ServiceLifetime.Singleton));
            }

            return services;
        }
    }
}