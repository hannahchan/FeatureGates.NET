namespace FeatureGates.UnitTests.Builder;

using System.Threading.Tasks;
using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class HalfGateUnitTests
{
    [Fact]
    public void When_ClosedByAction_Expect_FeatureGate()
    {
        // Arrange
        HalfGate halfGate = new HalfGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction, TestDelegates.Action);

        // Act
        FeatureGate featureGate = halfGate.WhenClosed(TestDelegates.Action);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunction, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.Action, featureGate.WhenOpened);
        Assert.Equal(TestDelegates.Action, featureGate.WhenClosed);
    }

    [Fact]
    public async Task When_ClosedByActionAsync_Expect_FeatureGateAsync()
    {
        // Arrange
        HalfGate halfGate = new HalfGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction, TestDelegates.Action);

        // Act
        FeatureGateAsync featureGate = halfGate.WhenClosed(TestDelegates.ActionAsync);

        // Assert
        Assert.Null(await Record.ExceptionAsync(() => featureGate.ControlledBy()));
        Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenOpened()));
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenClosed);
    }

    [Fact]
    public void When_ClosedByActionAsyncAndWhenOpenedIsNull_Expect_FeatureGateAsync()
    {
        // Arrange
        HalfGate halfGate = new HalfGate("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction, null);

        // Act
        FeatureGateAsync featureGate = halfGate.WhenClosed(TestDelegates.ActionAsync);

        // Assert
        Assert.NotNull(featureGate.ControlledBy);
        Assert.Null(featureGate.WhenOpened);
        Assert.Equal(TestDelegates.ActionAsync, featureGate.WhenClosed);
    }
}
