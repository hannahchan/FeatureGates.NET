namespace FeatureGates.UnitTests.Builder;

using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class BaseGateUnitTests
{
    [Fact]
    public void When_OpenedByAction_Expect_HalfGate()
    {
        // Arrange
        BaseGate baseGate = new BaseGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction);

        // Act
        HalfGate halfGate = baseGate.WhenOpened(TestDelegates.Action);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunction, halfGate.ControlledBy);
        Assert.Equal(TestDelegates.Action, halfGate.WhenOpened);
        Assert.Null((halfGate as FeatureGate).WhenClosed);
    }

    [Fact]
    public void When_OpenedByFunction_Expect_PartialResultGate()
    {
        // Arrange
        BaseGate baseGate = new BaseGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction);

        // Act
        PartialResultGate<string> partialResultGate = baseGate.WhenOpened(TestDelegates.Function<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunction, partialResultGate.ControlledBy);
        Assert.Equal(TestDelegates.Function<string>, partialResultGate.WhenOpened);
    }

    [Fact]
    public void When_OpenedByActionAsync_Expect_HalfGateAsync()
    {
        // Arrange
        BaseGate baseGate = new BaseGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction);

        // Act
        HalfGateAsync halfGate = baseGate.WhenOpened(TestDelegates.ActionAsync);

        // Assert
        Assert.NotNull(halfGate.ControlledBy);
        Assert.Equal(TestDelegates.ActionAsync, halfGate.WhenOpened);
        Assert.Null((halfGate as FeatureGateAsync).WhenClosed);
    }

    [Fact]
    public void When_OpenedByFunctionAsync_Expect_PartialResultGateAsync()
    {
        // Arrange
        BaseGate baseGate = new BaseGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction);

        // Act
        PartialResultGateAsync<string> partialResultGate = baseGate.WhenOpened(TestDelegates.FunctionAsync<string>);

        // Assert
        Assert.NotNull(partialResultGate.ControlledBy);
        Assert.Equal(TestDelegates.FunctionAsync<string>, partialResultGate.WhenOpened);
    }
}
