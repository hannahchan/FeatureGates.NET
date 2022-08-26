namespace FeatureGates.UnitTests.Builder;

using FeatureGates.Builder;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateBuilderUnitTests
{
    [Fact]
    public void When_ControlledByBooleanFunction_Expect_BaseGate()
    {
        // Arrange
        FeatureGateBuilder builder = new FeatureGateBuilder("myFeatureGateKey");

        // Act
        BaseGate baseGate = builder
            .WithHistogram()
            .WithFallbackOnException()
            .ControlledBy(TestDelegates.BooleanFunction);

        // Assert
        Assert.Equal("myFeatureGateKey", baseGate.Key);
        Assert.Equal(InstrumentType.Histogram, baseGate.InstrumentType);
        Assert.True(baseGate.FallbackOnException);
        Assert.Equal(TestDelegates.BooleanFunction, baseGate.ControlledBy);
    }

    [Fact]
    public void When_ControlledByBooleanFunctionAsync_Expect_BaseGateAsync()
    {
        // Arrange
        FeatureGateBuilder builder = new FeatureGateBuilder("myFeatureGateKey");

        // Act
        BaseGateAsync baseGate = builder
            .ControlledBy(TestDelegates.BooleanFunctionAsync);

        // Assert
        Assert.Equal("myFeatureGateKey", baseGate.Key);
        Assert.Equal(InstrumentType.Counter, baseGate.InstrumentType);
        Assert.False(baseGate.FallbackOnException);
        Assert.Equal(TestDelegates.BooleanFunctionAsync, baseGate.ControlledBy);
    }
}
