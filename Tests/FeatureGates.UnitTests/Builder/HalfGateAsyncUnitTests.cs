namespace FeatureGates.UnitTests.Builder;

using System;
using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class HalfGateAsyncUnitTests
{
    [Fact]
    public void When_ClosedByActionAsync_Expect_FeatureGateAsync()
    {
        // Arrange
        HalfGateAsync halfGate = new HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync, TestDelegates.ActionAsync);

        // Act
        FeatureGateAsync featureGate = halfGate.WhenClosed(TestDelegates.ActionAsync);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenOpened);
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenClosed);
    }

    [Fact]
    public void When_ClosedByAction_Expect_FeatureGate()
    {
        // Arrange
        HalfGateAsync halfGate = new HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync, TestDelegates.ActionAsync);

        // Act
        FeatureGateAsync featureGate = halfGate.WhenClosed(TestDelegates.Action);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenOpened);
        Assert.NotNull(featureGate.WhenClosed);
    }

    [Fact]
    public void When_ClosedByNullAction_Expect_FeatureGateAsync()
    {
        // Arrange
        HalfGateAsync halfGate = new HalfGateAsync("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync, TestDelegates.ActionAsync);

        // Act
        FeatureGateAsync featureGate = halfGate.WhenClosed(null as Action);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenOpened);
        Assert.Null(featureGate.WhenClosed);
    }
}
