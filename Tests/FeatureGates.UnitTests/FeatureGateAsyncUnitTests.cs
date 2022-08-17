namespace FeatureGates.UnitTests;

using System.Threading.Tasks;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateAsyncUnitTests
{
    [Theory]
    [InlineData(true, "Feature gate opened!")]
    [InlineData(false, "Feature gate closed.")]
    public async Task When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task WhenOpened() => Task.Run(() => result = "Feature gate opened!");
        Task WhenClosed() => Task.Run(() => result = "Feature gate closed.");

        // Act
        FeatureGateAsync featureGate = new FeatureGateAsync("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed);
        await featureGate.InvokeAsync();

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
    public async Task When_UsingFullConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task WhenOpened() => Task.Run(() => result = "Feature gate opened!");
        Task WhenClosed() => Task.Run(() => result = "Feature gate closed.");

        // Act
        FeatureGateAsync featureGate = new FeatureGateAsync("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);
        await featureGate.InvokeAsync();

        // Assert
        Assert.Equal("myFeatureGateKey", featureGate.Key);
        Assert.Equal(InstrumentType.None, featureGate.InstrumentType);
        Assert.True(featureGate.FallbackOnException);

        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
