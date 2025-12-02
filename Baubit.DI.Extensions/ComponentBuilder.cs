using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions
{
    /// <summary>
    /// Generic builder for creating a fully configured service of type <typeparamref name="T"/>
    /// from a collection of modules.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve from the built service provider.</typeparam>
    /// <remarks>
    /// This class extends <see cref="ComponentBuilder"/> to provide a convenient way to build
    /// a component and immediately resolve a specific service from the configured modules.
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = new ComponentBuilder&lt;IMyService&gt;();
    /// builder.WithModule&lt;MyModule, MyConfiguration&gt;(cfg => cfg.Value = "test");
    /// 
    /// var result = builder.Build();
    /// if (result.IsSuccess)
    /// {
    ///     IMyService service = result.Value;
    /// }
    /// </code>
    /// </example>
    public class ComponentBuilder<T> : ComponentBuilder where T : class
    {
        /// <summary>
        /// Builds the component and resolves a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the resolved service if successful,
        /// or failure information if the build or resolution fails.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// <list type="number">
        /// <item><description>Calls the base <see cref="ComponentBuilder.Build()"/> to create the component</description></item>
        /// <item><description>Creates a new <see cref="ServiceCollection"/></description></item>
        /// <item><description>Loads all modules from the component into the service collection</description></item>
        /// <item><description>Builds a service provider and resolves <typeparamref name="T"/></description></item>
        /// </list>
        /// </remarks>
        public new Result<T> Build()
        {
            return base.Build().Bind(component => Result.Try(() =>
            {
                var services = new ServiceCollection();
                foreach (var module in component)
                {
                    module.Load(services);
                }
                return services.BuildServiceProvider().GetRequiredService<T>();
            }));
        }
    }
}
