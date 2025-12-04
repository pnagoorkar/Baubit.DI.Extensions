using Baubit.Configuration;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Baubit.DI.Extensions
{
    /// <summary>
    /// Extension methods for building a service provider from <see cref="IComponent"/>.
    /// </summary>
    public static class ComponentBuilderExtensions
    {
        /// <summary>
        /// Adds a module to the service collection using configuration action.
        /// </summary>
        /// <typeparam name="TModule">The module type to add.</typeparam>
        /// <typeparam name="TConfiguration">The configuration type for the module.</typeparam>
        /// <param name="services">The service collection to add the module to.</param>
        /// <param name="configureAction">Action to configure the module.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service collection if successful,
        /// or failure information if the operation fails.
        /// </returns>
        public static Result<IServiceCollection> AddModule<TModule, TConfiguration>(this IServiceCollection services,
                                                                                    Action<TConfiguration> configureAction) where TModule : AModule<TConfiguration> where TConfiguration : AConfiguration
        {
            return ComponentBuilder.CreateNew()
                                   .WithModule<TModule, TConfiguration>(configureAction)
                                   .Build()
                                   .Bind(component => component.LoadModules(services));
        }

        /// <summary>
        /// Adds a module to the service collection using a configuration builder action.
        /// </summary>
        /// <typeparam name="TModule">The module type to add.</typeparam>
        /// <typeparam name="TConfiguration">The configuration type for the module.</typeparam>
        /// <param name="services">The service collection to add the module to.</param>
        /// <param name="configureAction">Action to configure the module using a configuration builder.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service collection if successful,
        /// or failure information if the operation fails.
        /// </returns>
        public static Result<IServiceCollection> AddModule<TModule, TConfiguration>(this IServiceCollection services,
                                                                                    Action<ConfigurationBuilder<TConfiguration>> configureAction) where TModule : AModule<TConfiguration> where TConfiguration : AConfiguration
        {
            return ComponentBuilder.CreateNew()
                                   .WithModule<TModule, TConfiguration>(configureAction)
                                   .Build()
                                   .Bind(component => component.LoadModules(services));
        }

        private static Result<IServiceCollection> LoadModules(this IEnumerable<IModule> modules, IServiceCollection services = null)
        {
            return Result.Try(() => 
            {
                if (services == null) services = new ServiceCollection();
                foreach (var module in modules)
                {
                    module.Load(services);
                }
                return services;
            });
        }
        /// <summary>
        /// Builds a service provider from the component's modules.
        /// </summary>
        /// <param name="component">The component containing modules.</param>
        /// <param name="services">Optional existing service collection to add modules to. If null, a new collection is created.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this IComponent component, IServiceCollection services = null)
        {
            return component.LoadModules(services).Bind(s => Result.Try<IServiceProvider>(() => s.BuildServiceProvider()));
        }

        /// <summary>
        /// Builds a service provider from a component result.
        /// </summary>
        /// <param name="result">The result containing the component.</param>
        /// <param name="services">Optional existing service collection to add modules to. If null, a new collection is created.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this Result<IComponent> result, IServiceCollection services = null)
        {
            return result.Bind(component => component.BuildServiceProvider(services));
        }

        /// <summary>
        /// Builds the component and creates a service provider.
        /// </summary>
        /// <param name="result">The result containing the component builder.</param>
        /// <param name="services">Optional existing service collection to add modules to. If null, a new collection is created.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the service provider if successful,
        /// or failure information if the build fails.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing the returned <see cref="IServiceProvider"/>.
        /// </remarks>
        public static Result<IServiceProvider> BuildServiceProvider(this Result<ComponentBuilder> result, IServiceCollection services = null)
        {
            return result.Build().BuildServiceProvider(services);
        }
    }
}
