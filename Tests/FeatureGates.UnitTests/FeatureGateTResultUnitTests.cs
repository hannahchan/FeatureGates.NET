namespace FeatureGates.UnitTests;

using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateTResultUnitTests
{
    [Theory]
    [InlineData(true, "Feature gate opened!")]
    [InlineData(false, "Feature gate closed.")]
    public void When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        bool ControlledBy() => isOpened;
        string WhenOpened() => "Feature gate opened!";
        string WhenClosed() => "Feature gate closed.";

        // Act
        FeatureGate<string> featureGate = new FeatureGate<string>("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed);
        string result = featureGate.Invoke();

        // Assert
        Assert.Equal("myFeatureGateKey", featureGate.Key);
        Assert.Equal(InstrumentType.Counter, featureGate.InstrumentType);
        Assert.False(featureGate.FallbackOnException);

        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Single(meterListener.Measurements);
    }

    [Theory]
    [InlineData(true, "Feature gate opened!")]
    [InlineData(false, "Feature gate closed.")]
    public void When_UsingFullConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        bool ControlledBy() => isOpened;
        string WhenOpened() => "Feature gate opened!";
        string WhenClosed() => "Feature gate closed.";

        // Act
        FeatureGate<string> featureGate = new FeatureGate<string>("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);
        string result = featureGate.Invoke();

        // Assert
        Assert.Equal("myFeatureGateKey", featureGate.Key);
        Assert.Equal(InstrumentType.None, featureGate.InstrumentType);
        Assert.True(featureGate.FallbackOnException);

        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
