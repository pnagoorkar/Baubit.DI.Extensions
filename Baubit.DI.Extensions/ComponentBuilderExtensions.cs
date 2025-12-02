using Baubit.DI;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions
{
    /// <summary>
    /// Extension methods for building and resolving services from <see cref="IComponent"/>.
    /// </summary>
    public static class ComponentBuilderExtensions
    {
        /// <summary>
        /// Builds a service of type <typeparamref name="T"/> from the component's modules.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <param name="component">The component containing modules.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the resolved service if successful,
        /// or failure information if resolution fails.
        /// </returns>
        public static Result<T> Build<T>(this IComponent component) where T : class
        {
            return Result.Try(() =>
            {
                var services = new ServiceCollection();
                foreach (var module in component)
                {
                    module.Load(services);
                }
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    return serviceProvider.GetRequiredService<T>();
                }
            });
        }

        /// <summary>
        /// Builds a service of type <typeparamref name="T"/> from a component result.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <param name="result">The result containing the component.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the resolved service if successful,
        /// or failure information if the build or resolution fails.
        /// </returns>
        public static Result<T> Build<T>(this Result<IComponent> result) where T : class
        {
            return result.Bind(component => Build<T>(component));
        }

        /// <summary>
        /// Builds the component and resolves a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <param name="result">The result containing the component builder.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the resolved service if successful,
        /// or failure information if the build or resolution fails.
        /// </returns>
        public static Result<T> Build<T>(this Result<ComponentBuilder> result) where T : class
        {
            return result.Build().Bind(component => Build<T>(component));
        }
    }
}
