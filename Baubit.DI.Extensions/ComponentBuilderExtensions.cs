using System;
using Baubit.DI;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions
{
    /// <summary>
    /// Extension methods for building a service provider from <see cref="IComponent"/>.
    /// </summary>
    public static class ComponentBuilderExtensions
    {
        /// <summary>
        /// Builds a service provider from the component's modules.
        /// </summary>
        /// <param name="component">The component containing modules.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this IComponent component)
        {
            return Result.Try<IServiceProvider>(() =>
            {
                var services = new ServiceCollection();
                foreach (var module in component)
                {
                    module.Load(services);
                }
                return services.BuildServiceProvider();
            });
        }

        /// <summary>
        /// Builds a service provider from a component result.
        /// </summary>
        /// <param name="result">The result containing the component.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this Result<IComponent> result)
        {
            return result.Bind(component => component.BuildServiceProvider());
        }

        /// <summary>
        /// Builds the component and creates a service provider.
        /// </summary>
        /// <param name="result">The result containing the component builder.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this Result<ComponentBuilder> result)
        {
            return result.Build().BuildServiceProvider();
        }
    }
}
