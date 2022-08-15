namespace FeatureGates.UnitTests;

using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateTResultUnitTests
{
    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public void When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        bool ControlledBy() => isOpened;
        string WhenOpened() => "Feature gated opened!";
        string WhenClosed() => "Feature gated closed.";

        // Act
        string result = new FeatureGate<string>("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed).Invoke();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Single(meterListener.Measurements);
    }

    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public void When_UsingFullConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        bool ControlledBy() => isOpened;
        string WhenOpened() => "Feature gated opened!";
        string WhenClosed() => "Feature gated closed.";

        // Act
        string result = new FeatureGate<string>("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed).Invoke();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
