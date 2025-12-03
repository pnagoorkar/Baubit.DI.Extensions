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
        public void BuildServiceProvider_OnIComponent_ReturnsServiceProvider()
        {
            // Arrange
            var componentResult = ComponentBuilder.CreateNew()
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "TestValue")
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
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "ResultValue")
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
                .WithModule<TestModule, TestConfiguration>(cfg => cfg.Value = "BuilderValue")
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

    public class TestConfiguration : AConfiguration
    {
        public string Value { get; set; } = string.Empty;
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

    #endregion
}
