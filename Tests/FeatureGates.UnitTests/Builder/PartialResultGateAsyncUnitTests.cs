namespace FeatureGates.UnitTests.Builder;

using System.Threading.Tasks;
using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class PartialResultGateAsyncUnitTests
{
    [Fact]
    public void When_ClosedByFunctionAsync_Expect_FeatureGateAsync()
    {
        // Arrange
        PartialResultGateAsync<string> resultGate =
            new PartialResultGateAsync<string>("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync, TestDelegates.FunctionAsync<string>);

        // Act
        FeatureGateAsync<string> featureGate = resultGate.WhenClosed(TestDelegates.FunctionAsync<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.FunctionAsync<string>, featureGate.WhenOpened);
        Assert.Equal(TestDelegates.FunctionAsync<string>, featureGate.WhenClosed);
    }

    [Fact]
    public async Task When_ClosedByFunction_Expect_FeatureGateAsync()
    {
        // Arrange
        PartialResultGateAsync<string> resultGate =
            new PartialResultGateAsync<string>("myFeatureGateKey", InstrumentType.None, false, TestDelegates.BooleanFunctionAsync, TestDelegates.FunctionAsync<string>);

        // Act
        FeatureGateAsync<string> featureGate = resultGate.WhenClosed(TestDelegates.Function<string>);

        // Assert
        Assert.Equal(TestDelegates.BooleanFunctionAsync, featureGate.ControlledBy);
        Assert.Equal(TestDelegates.FunctionAsync<string>, featureGate.WhenOpened);
        Assert.Null(await Record.ExceptionAsync(() => featureGate.WhenClosed()));
    }
}
