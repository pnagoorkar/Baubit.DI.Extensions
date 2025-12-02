using Baubit.DI;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Baubit.DI.Extensions.Test.ComponentBuilderExtensionsTests
{
    /// <summary>
    /// Unit tests for <see cref="ComponentBuilderExtensions"/>
    /// </summary>
    public class Test
    {
        [Fact]
        public void Build_OnResultComponentBuilder_WithValidModule_ReturnsResolvedService()
        {
            // Arrange & Act
            var result = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "TestValue")
                .Build<ITestService>();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<ITestService>(result.Value);
            Assert.Equal("TestValue", result.Value.GetValue());
        }

        [Fact]
        public void Build_OnResultComponentBuilder_WithNoModules_ReturnsFailure()
        {
            // Arrange & Act
            var result = ComponentBuilder.CreateNew()
                .Build<ITestService>();

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public void Build_OnResultComponentBuilder_WithMultipleModules_AllModulesAreLoaded()
        {
            // Arrange & Act
            var result = ComponentBuilder.CreateNew()
                .WithModule<DependencyModule, DependencyConfiguration>(cfg => cfg.DependencyValue = "DependencyData")
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "MainValue")
                .Build<ITestService>();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.NotNull(result.Value);
            Assert.Equal("MainValue", result.Value.GetValue());
        }

        [Fact]
        public void Build_OnIComponent_ReturnsResolvedService()
        {
            // Arrange
            var componentResult = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "DirectValue")
                .Build();
            Assert.True(componentResult.IsSuccess);
            
            // Act
            var result = componentResult.Value.Build<ITestService>();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.Equal("DirectValue", result.Value.GetValue());
        }

        [Fact]
        public void Build_OnResultIComponent_Success_ReturnsResolvedService()
        {
            // Arrange
            var componentResult = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "ResultValue")
                .Build();

            // Act - call the Result<IComponent> extension method
            var result = componentResult.Build<ITestService>();

            // Assert
            Assert.True(result.IsSuccess, $"Build failed: {string.Join(", ", result.Errors)}");
            Assert.Equal("ResultValue", result.Value.GetValue());
        }

        [Fact]
        public void Build_OnResultIComponent_Failure_ReturnsFailure()
        {
            // Arrange - create a failed Result<IComponent>
            var failedResult = Result.Fail<IComponent>("Test failure");

            // Act
            var result = failedResult.Build<ITestService>();

            // Assert
            Assert.True(result.IsFailed);
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
