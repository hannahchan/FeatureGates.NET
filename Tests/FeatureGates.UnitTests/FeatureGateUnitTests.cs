namespace FeatureGates.UnitTests;

using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateUnitTests
{
    [Theory]
    [InlineData(true, "Feature gate opened!")]
    [InlineData(false, "Feature gate closed.")]
    public void When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        bool ControlledBy() => isOpened;
        void WhenOpened() => result = "Feature gate opened!";
        void WhenClosed() => result = "Feature gate closed.";

        // Act
        FeatureGate featureGate = new FeatureGate("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed);
        featureGate.Invoke();

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

        string result = string.Empty;

        bool ControlledBy() => isOpened;
        void WhenOpened() => result = "Feature gate opened!";
        void WhenClosed() => result = "Feature gate closed.";

        // Act
        FeatureGate featureGate = new FeatureGate("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);
        featureGate.Invoke();

        // Assert
        Assert.Equal("myFeatureGateKey", featureGate.Key);
        Assert.Equal(InstrumentType.None, featureGate.InstrumentType);
        Assert.True(featureGate.FallbackOnException);

        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
