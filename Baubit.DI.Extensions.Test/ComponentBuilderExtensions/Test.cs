using Baubit.Configuration;
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
        #region AddModule Tests

        [Fact]
        public void AddModule_WithConfigurationAction_ReturnsServiceCollectionWithModule()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddModule<TestModule, TestConfiguration>(cfg => cfg.Value = "TestValue", cfg => new TestModule(cfg));

            // Assert
            Assert.Same(services, result);
            var provider = result.BuildServiceProvider();
            var service = provider.GetService<ITestService>();
            Assert.NotNull(service);
            Assert.Equal("TestValue", service.GetValue());
        }

        [Fact]
        public void AddModule_WithConfigurationBuilderAction_ReturnsServiceCollectionWithModule()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddModule<TestModule, TestConfiguration>(builder => builder.WithRawJsonStrings("{\"Value\": \"BuilderValue\"}"), cfg => new TestModule(cfg));

            // Assert
            Assert.Same(services, result);
            var provider = result.BuildServiceProvider();
            var service = provider.GetService<ITestService>();
            Assert.NotNull(service);
            Assert.Equal("BuilderValue", service.GetValue());
        }

        [Fact]
        public void AddModule_WithExistingServices_AddsToExistingCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IExistingService, ExistingService>();

            // Act
            var result = services.AddModule<TestModule, TestConfiguration>(cfg => cfg.Value = "TestValue", cfg => new TestModule(cfg));

            // Assert
            Assert.Same(services, result);
            var provider = result.BuildServiceProvider();
            var existingService = provider.GetService<IExistingService>();
            var testService = provider.GetService<ITestService>();
            Assert.NotNull(existingService);
            Assert.NotNull(testService);
        }

        [Fact]
        public void AddModule_AllowsFluentChaining()
        {
            // Arrange & Act
            var provider = new ServiceCollection()
                .AddModule<TestModule, TestConfiguration>(cfg => cfg.Value = "First", cfg => new TestModule(cfg))
                .AddSingleton<IExistingService, ExistingService>()
                .BuildServiceProvider();

            // Assert
            var testService = provider.GetService<ITestService>();
            var existingService = provider.GetService<IExistingService>();
            Assert.NotNull(testService);
            Assert.NotNull(existingService);
            Assert.Equal("First", testService.GetValue());
        }

        #endregion

        #region BuildServiceProvider Tests

        [Fact]
        public void BuildServiceProvider_OnIComponent_ReturnsServiceProvider()
        {
            // Arrange
            var componentResult = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "TestValue", cfg => new TestModule(cfg))
                .Build();
            Assert.True(componentResult.IsSuccess);

            // Act
            var result = componentResult.Value.BuildServiceProvider();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            var service = result.Value.GetService<ITestService>();
            Assert.NotNull(service);
            Assert.Equal("TestValue", service.GetValue());
        }

        [Fact]
        public void BuildServiceProvider_OnResultIComponent_Success_ReturnsServiceProvider()
        {
            // Arrange
            var componentResult = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "ResultValue", cfg => new TestModule(cfg))
                .Build();

            // Act
            var result = componentResult.BuildServiceProvider();

            // Assert
            Assert.True(result.IsSuccess);
            var service = result.Value.GetService<ITestService>();
            Assert.NotNull(service);
            Assert.Equal("ResultValue", service.GetValue());
        }

        [Fact]
        public void BuildServiceProvider_OnResultIComponent_Failure_ReturnsFailure()
        {
            // Arrange
            var failedResult = Result.Fail<IComponent>("Test failure");

            // Act
            var result = failedResult.BuildServiceProvider();

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public void BuildServiceProvider_OnResultComponentBuilder_ReturnsServiceProvider()
        {
            // Arrange & Act
            var result = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "BuilderValue", cfg => new TestModule(cfg))
                .BuildServiceProvider();

            // Assert
            Assert.True(result.IsSuccess);
            var service = result.Value.GetService<ITestService>();
            Assert.NotNull(service);
            Assert.Equal("BuilderValue", service.GetValue());
        }

        [Fact]
        public void BuildServiceProvider_OnResultComponentBuilder_Failure_ReturnsFailure()
        {
            // Arrange
            var failedResult = Result.Fail<ComponentBuilder>("Builder failure");

            // Act
            var result = failedResult.BuildServiceProvider();

            // Assert
            Assert.True(result.IsFailed);
        }

        #endregion
    }

    #region Test Helpers

    public interface ITestService
    {
        string GetValue();
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

    public interface IExistingService
    {
        string GetName();
    }

    public class ExistingService : IExistingService
    {
        public string GetName() => "ExistingService";
    }

    public class TestConfiguration : Configuration
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestModule : Module<TestConfiguration>
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

    #endregion
}
