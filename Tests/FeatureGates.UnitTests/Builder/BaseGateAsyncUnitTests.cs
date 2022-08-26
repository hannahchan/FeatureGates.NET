namespace FeatureGates.UnitTests.Builder;

using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class BaseGateAsyncUnitTests
{
    [Fact]
    public void When_OpenedByAction_Expect_HalfGateAsync()
    {
        // Arrange
        BaseGateAsync baseGate = new BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync);

        // Act
        HalfGateAsync halfGate = baseGate.WhenOpened(TestDelegates.Action);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, halfGate.ControlledBy);
        Assert.NotNull(halfGate.WhenOpened);
        Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
    }

    [Fact]
    public void When_OpenedByFunction_Expect_PartialResultGateAsync()
    {
        // Arrange
        BaseGateAsync baseGate = new BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync);

        // Act
        PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(TestDelegates.Function<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, partialResultGate.ControlledBy);
        Assert.NotNull(partialResultGate.WhenOpened);
    }

    [Fact]
    public void When_OpenedByActionAsync_Expect_HalfGateAsync()
    {
        // Arrange
        BaseGateAsync baseGate = new BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync);

        // Act
        HalfGateAsync halfGate = baseGate.WhenOpened(TestDelegates.ActionAsync);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, halfGate.ControlledBy);
        Assert.Equal(TestDelegates.ActionAsync, halfGate.WhenOpened);
        Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
    }

    [Fact]
    public void When_OpenedByFunctionAsync_Expect_PartialResultGateAsync()
    {
        // Arrange
        BaseGateAsync baseGate = new BaseGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync);

        // Act
        PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(TestDelegates.FunctionAsync<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, partialResultGate.ControlledBy);
        Assert.Equal(TestDelegates.FunctionAsync<string>, partialResultGate.WhenOpened);
    }
}
