using Baubit.DI;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions.Test.ComponentBuilderTests
{
    /// <summary>
    /// Unit tests for <see cref="Baubit.DI.Extensions.ComponentBuilder{T}"/>
    /// </summary>
    public class Test
    {
        [Fact]
        public void Build_WithValidModule_ReturnsResolvedService()
        {
            // Arrange
            var builder = new Baubit.DI.Extensions.ComponentBuilder<ITestService>();
            var withModuleResult = builder.WithModule<TestModule, TestConfiguration>(cfg =>
            {
                cfg.Value = "TestValue";
            });

            // Act
            Assert.True(withModuleResult.IsSuccess, $"WithModule failed: {string.Join(", ", withModuleResult.Errors)}");
            var result = builder.Build();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<ITestService>(result.Value);
            Assert.Equal("TestValue", result.Value.GetValue());
        }

        [Fact]
        public void Build_WithNoModules_ReturnsFailure_WhenServiceCannotBeResolved()
        {
            // Arrange
            var builder = new Baubit.DI.Extensions.ComponentBuilder<ITestService>();

            // Act
            var result = builder.Build();

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public void Build_WithMultipleModules_AllModulesAreLoaded()
        {
            // Arrange
            var builder = new Baubit.DI.Extensions.ComponentBuilder<ITestService>();
            var result1 = builder.WithModule<DependencyModule, DependencyConfiguration>(cfg =>
            {
                cfg.DependencyValue = "DependencyData";
            });
            Assert.True(result1.IsSuccess, $"WithModule DependencyModule failed: {string.Join(", ", result1.Errors)}");
            
            var result2 = builder.WithModule<TestModule, TestConfiguration>(cfg =>
            {
                cfg.Value = "MainValue";
            });
            Assert.True(result2.IsSuccess, $"WithModule TestModule failed: {string.Join(", ", result2.Errors)}");

            // Act
            var result = builder.Build();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.NotNull(result.Value);
            Assert.Equal("MainValue", result.Value.GetValue());
        }
    }

    #region Test Helpers

    public interface ITestService
    {
        string GetValue();
    }

    public interface IDependencyService
    {
        string GetDependencyValue();
    }

    public class TestService : ITestService
    {
        private readonly string _value;

        public TestService(string value)
        {
            _value = value;
        }

        public string GetValue() => _value;
    }

    public class DependencyService : IDependencyService
    {
        private readonly string _value;

        public DependencyService(string value)
        {
            _value = value;
        }

        public string GetDependencyValue() => _value;
    }

    public class TestConfiguration : AConfiguration
    {
        public string Value { get; set; } = string.Empty;
    }

    public class DependencyConfiguration : AConfiguration
    {
        public string DependencyValue { get; set; } = string.Empty;
    }

    public class TestModule : AModule<TestConfiguration>
    {
        public TestModule(TestConfiguration configuration, List<IModule>? nestedModules = null)
            : base(configuration, nestedModules)
        {
        }

        public override void Load(IServiceCollection services)
        {
            services.AddSingleton<ITestService>(new TestService(Configuration.Value));
            base.Load(services);
        }
    }

    public class DependencyModule : AModule<DependencyConfiguration>
    {
        public DependencyModule(DependencyConfiguration configuration, List<IModule>? nestedModules = null)
            : base(configuration, nestedModules)
        {
        }

        public override void Load(IServiceCollection services)
        {
            services.AddSingleton<IDependencyService>(new DependencyService(Configuration.DependencyValue));
            base.Load(services);
        }
    }

    #endregion
}
