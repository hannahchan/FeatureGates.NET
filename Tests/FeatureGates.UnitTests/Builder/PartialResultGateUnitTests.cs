namespace FeatureGates.UnitTests.Builder;

using System.Threading.Tasks;
using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class PartialResultGateUnitTests
{
    [Fact]
    public void When_ClosedByFunction_Expect_FeatureGate()
    {
        // Arrange
        PartialResultGate<string> resultGate =
            new PartialResultGate<string>("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction, TestDelegates.Function<string>);

        // Act
        FeatureGate<string> featureGate = resultGate.WhenClosed(TestDelegates.Function<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunction, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.Function<string>, featureGate.WhenOpened);
        Assert.Equal(TestDelegates.Function<string>, featureGate.WhenClosed);
    }

    [Fact]
    public async Task When_ClosedByFunctionAsync_Expect_FeatureGateAsync()
    {
        // Arrange
        PartialResultGate<string> resultGate =
            new PartialResultGate<string>("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunction, TestDelegates.Function<string>);

        // Act
        FeatureGateAsync<string> featureGate = resultGate.WhenClosed(TestDelegates.FunctionAsync<string>);

        // Assert
        Assert.Null(await Record.ExceptionAsync(() => featureGate.ControlledBy()));
        Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenOpened()));
        Assert.Equal(TestDelegates.FunctionAsync<string>, featureGate.WhenClosed);
    }
}
