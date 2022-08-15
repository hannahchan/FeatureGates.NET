namespace FeatureGates.UnitTests;

using System.Threading.Tasks;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateAsyncUnitTests
{
    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public async Task When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task WhenOpened() => Task.Run(() => result = "Feature gated opened!");
        Task WhenClosed() => Task.Run(() => result = "Feature gated closed.");

        // Act
        await new FeatureGateAsync("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed).InvokeAsync();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Single(meterListener.Measurements);
    }

    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public async Task When_UsingFullConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        string result = string.Empty;

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task WhenOpened() => Task.Run(() => result = "Feature gated opened!");
        Task WhenClosed() => Task.Run(() => result = "Feature gated closed.");

        // Act
        await new FeatureGateAsync("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed).InvokeAsync();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
