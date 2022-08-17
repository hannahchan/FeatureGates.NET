namespace FeatureGates.UnitTests;

using System;
using System.Threading.Tasks;
using Xunit;

public class FeatureGateBuilderUnitTests
{
    public class WithKey
    {
        [Fact]
        public void When_InvokingStaticMethodWithKey_Expect_FeatureGateBuilder()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGate.Builder builder = FeatureGate.WithKey(featureGateKey);

            // Assert
            Assert.Equal(featureGateKey, builder.Key);
        }
    }

    public class Builder
    {
        [Fact]
        public void When_ControlledByBooleanFunction_Expect_BaseGate()
        {
            // Arrange
            FeatureGate.Builder builder = new FeatureGate.Builder("myFeatureGateKey");

            // Act
            FeatureGate.BaseGate baseGate = builder
                .WithHistogram()
                .WithFallbackOnException()
                .ControlledBy(Test.BooleanFunction);

            // Assert
            Assert.Equal("myFeatureGateKey", baseGate.Key);
            Assert.Equal(InstrumentType.Histogram, baseGate.InstrumentType);
            Assert.True(baseGate.FallbackOnException);
            Assert.Equal(Test.BooleanFunction, baseGate.ControlledBy);
        }

        [Fact]
        public void When_ControlledByBooleanFunctionAsync_Expect_BaseGateAsync()
        {
            // Arrange
            FeatureGate.Builder builder = new FeatureGate.Builder("myFeatureGateKey");

            // Act
            FeatureGate.BaseGateAsync baseGate = builder
                .ControlledBy(Test.BooleanFunctionAsync);

            // Assert
            Assert.Equal("myFeatureGateKey", baseGate.Key);
            Assert.Equal(InstrumentType.Counter, baseGate.InstrumentType);
            Assert.False(baseGate.FallbackOnException);
            Assert.Equal(Test.BooleanFunctionAsync, baseGate.ControlledBy);
        }
    }

    public class BaseGate
    {
        [Fact]
        public void When_OpenedByAction_Expect_HalfGate()
        {
            // Arrange
            FeatureGate.BaseGate baseGate = new FeatureGate.BaseGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction);

            // Act
            FeatureGate.HalfGate halfGate = baseGate.WhenOpened(Test.Action);

            // Assert
            Assert.Equal(Test.BooleanFunction, halfGate.ControlledBy);
            Assert.Equal(Test.Action, halfGate.WhenOpened);
            Assert.Null((halfGate as FeatureGate).WhenClosed);
        }

        [Fact]
        public void When_OpenedByFunction_Expect_PartialResultGate()
        {
            // Arrange
            FeatureGate.BaseGate baseGate = new FeatureGate.BaseGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction);

            // Act
            FeatureGate.PartialResultGate<string> partialResultGate = baseGate.WhenOpened(Test.Function<string>);

            // Assert
            Assert.Equal(Test.BooleanFunction, partialResultGate.ControlledBy);
            Assert.Equal(Test.Function<string>, partialResultGate.WhenOpened);
        }

        [Fact]
        public void When_OpenedByActionAsync_Expect_HalfGateAsync()
        {
            // Arrange
            FeatureGate.BaseGate baseGate = new FeatureGate.BaseGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction);

            // Act
            FeatureGate.HalfGateAsync halfGate = baseGate.WhenOpened(Test.ActionAsync);

            // Assert
            Assert.NotNull(halfGate.ControlledBy);
            Assert.Equal(Test.ActionAsync, halfGate.WhenOpened);
            Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
        }

        [Fact]
        public void When_OpenedByFunctionAsync_Expect_PartialResultGateAsync()
        {
            // Arrange
            FeatureGate.BaseGate baseGate = new FeatureGate.BaseGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction);

            // Act
            FeatureGate.PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(Test.FunctionAsync<string>);

            // Assert
            Assert.NotNull(partialResultGate.ControlledBy);
            Assert.Equal(Test.FunctionAsync<string>, partialResultGate.WhenOpened);
        }
    }

    public class BaseGateAsync
    {
        [Fact]
        public void When_OpenedByAction_Expect_HalfGateAsync()
        {
            // Arrange
            FeatureGate.BaseGateAsync baseGate = new FeatureGate.BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync);

            // Act
            FeatureGate.HalfGateAsync halfGate = baseGate.WhenOpened(Test.Action);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, halfGate.ControlledBy);
            Assert.NotNull(halfGate.WhenOpened);
            Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
        }

        [Fact]
        public void When_OpenedByFunction_Expect_PartialResultGateAsync()
        {
            // Arrange
            FeatureGate.BaseGateAsync baseGate = new FeatureGate.BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync);

            // Act
            FeatureGate.PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(Test.Function<string>);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, partialResultGate.ControlledBy);
            Assert.NotNull(partialResultGate.WhenOpened);
        }

        [Fact]
        public void When_OpenedByActionAsync_Expect_HalfGateAsync()
        {
            // Arrange
            FeatureGate.BaseGateAsync baseGate = new FeatureGate.BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync);

            // Act
            FeatureGate.HalfGateAsync halfGate = baseGate.WhenOpened(Test.ActionAsync);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, halfGate.ControlledBy);
            Assert.Equal(Test.ActionAsync, halfGate.WhenOpened);
            Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
        }

        [Fact]
        public void When_OpenedByFunctionAsync_Expect_PartialResultGateAsync()
        {
            // Arrange
            FeatureGate.BaseGateAsync baseGate = new FeatureGate.BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync);

            // Act
            FeatureGate.PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(Test.FunctionAsync<string>);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, partialResultGate.ControlledBy);
            Assert.Equal(Test.FunctionAsync<string>, partialResultGate.WhenOpened);
        }
    }

    public class HalfGate
    {
        [Fact]
        public void When_ClosedByAction_Expect_FeatureGate()
        {
            // Arrange
            FeatureGate.HalfGate halfGate = new FeatureGate.HalfGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction, Test.Action);

            // Act
            FeatureGate featureGate = halfGate.WhenClosed(Test.Action);

            // Assert
            Assert.Equal(Test.BooleanFunction, featureGate.ControlledBy);
            Assert.Equal(Test.Action, featureGate.WhenOpened);
            Assert.Equal(Test.Action, featureGate.WhenClosed);
        }

        [Fact]
        public async Task When_ClosedByActionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.HalfGate halfGate = new FeatureGate.HalfGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction, Test.Action);

            // Act
            FeatureGateAsync featureGate = halfGate.WhenClosed(Test.ActionAsync);

            // Assert
            Assert.Null(await Record.ExceptionAsync(() => featureGate.ControlledBy()));
            Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenOpened()));
            Assert.Equal(Test.ActionAsync, featureGate.WhenClosed);
        }

        [Fact]
        public void When_ClosedByActionAsyncAndWhenOpenedIsNull_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.HalfGate halfGate = new FeatureGate.HalfGate("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction, null);

            // Act
            FeatureGateAsync featureGate = halfGate.WhenClosed(Test.ActionAsync);

            // Assert
            Assert.NotNull(featureGate.ControlledBy);
            Assert.Null(featureGate.WhenOpened);
            Assert.Equal(Test.ActionAsync, featureGate.WhenClosed);
        }
    }

    public class HalfGateAsync
    {
        [Fact]
        public void When_ClosedByActionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.HalfGateAsync halfGate = new FeatureGate.HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync, Test.ActionAsync);

            // Act
            FeatureGateAsync featureGate = halfGate.WhenClosed(Test.ActionAsync);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, featureGate.ControlledBy);
            Assert.Equal(Test.ActionAsync, featureGate.WhenOpened);
            Assert.Equal(Test.ActionAsync, featureGate.WhenClosed);
        }

        [Fact]
        public void When_ClosedByAction_Expect_FeatureGate()
        {
            // Arrange
            FeatureGate.HalfGateAsync halfGate = new FeatureGate.HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync, Test.ActionAsync);

            // Act
            FeatureGateAsync featureGate = halfGate.WhenClosed(Test.Action);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, featureGate.ControlledBy);
            Assert.Equal(Test.ActionAsync, featureGate.WhenOpened);
            Assert.NotNull(featureGate.WhenClosed);
        }

        [Fact]
        public void When_ClosedByNullAction_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.HalfGateAsync halfGate = new FeatureGate.HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync, Test.ActionAsync);

            // Act
            FeatureGateAsync featureGate = halfGate.WhenClosed(null as Action);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, featureGate.ControlledBy);
            Assert.Equal(Test.ActionAsync, featureGate.WhenOpened);
            Assert.Null(featureGate.WhenClosed);
        }
    }

    public class PartialResultGate
    {
        [Fact]
        public void When_ClosedByFunction_Expect_FeatureGate()
        {
            // Arrange
            FeatureGate.PartialResultGate<string> resultGate =
                new FeatureGate.PartialResultGate<string>("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction, Test.Function<string>);

            // Act
            FeatureGate<string> featureGate = resultGate.WhenClosed(Test.Function<string>);

            // Assert
            Assert.Equal(Test.BooleanFunction, featureGate.ControlledBy);
            Assert.Equal(Test.Function<string>, featureGate.WhenOpened);
            Assert.Equal(Test.Function<string>, featureGate.WhenClosed);
        }

        [Fact]
        public async Task When_ClosedByFunctionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.PartialResultGate<string> resultGate =
                new FeatureGate.PartialResultGate<string>("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunction, Test.Function<string>);

            // Act
            FeatureGateAsync<string> featureGate = resultGate.WhenClosed(Test.FunctionAsync<string>);

            // Assert
            Assert.Null(await Record.ExceptionAsync(() => featureGate.ControlledBy()));
            Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenOpened()));
            Assert.Equal(Test.FunctionAsync<string>, featureGate.WhenClosed);
        }
    }

    public class PartialResultGateAsync
    {
        [Fact]
        public void When_ClosedByFunctionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.PartialResultGateAsync<string> resultGate =
                new FeatureGate.PartialResultGateAsync<string>("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync, Test.FunctionAsync<string>);

            // Act
            FeatureGateAsync<string> featureGate = resultGate.WhenClosed(Test.FunctionAsync<string>);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, featureGate.ControlledBy);
            Assert.Equal(Test.FunctionAsync<string>, featureGate.WhenOpened);
            Assert.Equal(Test.FunctionAsync<string>, featureGate.WhenClosed);
        }

        [Fact]
        public async Task When_ClosedByFunction_Expect_FeatureGateAsync()
        {
            // Arrange
            FeatureGate.PartialResultGateAsync<string> resultGate =
                new FeatureGate.PartialResultGateAsync<string>("myFeatureGateKey", InstrumentType.None, false, Test.BooleanFunctionAsync, Test.FunctionAsync<string>);

            // Act
            FeatureGateAsync<string> featureGate = resultGate.WhenClosed(Test.Function<string>);

            // Assert
            Assert.Equal(Test.BooleanFunctionAsync, featureGate.ControlledBy);
            Assert.Equal(Test.FunctionAsync<string>, featureGate.WhenOpened);
            Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenClosed()));
        }
    }

    private static class Test
    {
        public static bool BooleanFunction()
        {
            return new Random().NextDouble() >= 0.5;
        }

        public static Task<bool> BooleanFunctionAsync()
        {
            return Task.FromResult(new Random().NextDouble() >= 0.5);
        }

        public static void Action()
        {
            // Do nothing
        }

        public static TResult Function<TResult>()
        {
            return default;
        }

        public static Task ActionAsync()
        {
            return Task.CompletedTask;
        }

        public static Task<TResult> FunctionAsync<TResult>()
        {
            return Task.FromResult<TResult>(default);
        }
    }
}
