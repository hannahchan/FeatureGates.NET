namespace FeatureGates.UnitTests;

using System;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateUnitTests
{
    [Fact]
    public void When_InvokingStaticMethodWithKey_Expect_FeatureGateBuilder()
    {
        // Arrange
        string featureGateKey = Guid.NewGuid().ToString();

        // Act
        FeatureGateBuilder builder = FeatureGate.WithKey(featureGateKey);

        // Assert
        Assert.Equal(featureGateKey, builder.Key);
    }

    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public void When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        bool ControlledBy() => isOpened;
        void WhenOpened() => result = "Feature gated opened!";
        void WhenClosed() => result = "Feature gated closed.";

        // Act
        new FeatureGate("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed).Invoke();

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

        string result = string.Empty;

        bool ControlledBy() => isOpened;
        void WhenOpened() => result = "Feature gated opened!";
        void WhenClosed() => result = "Feature gated closed.";

        // Act
        new FeatureGate("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed).Invoke();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
