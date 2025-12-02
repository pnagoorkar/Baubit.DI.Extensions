using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions
{
    public class ComponentBuilder<T> : ComponentBuilder where T : class
    {
        public new Result<T> Build()
        {
            return base.Build().Bind(component => Result.Try(() =>
            {
                var services = new ServiceCollection().AddSingleton<T>();
                foreach (var module in component)
                {
                    module.Load(services);
                }
                return services.BuildServiceProvider().GetRequiredService<T>();
            }));
        }
    }
}
