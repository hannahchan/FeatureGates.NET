namespace FeatureGates.UnitTests;

using System.Threading.Tasks;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateAsyncTResultUnitTests
{
    [Theory]
    [InlineData(true, "Feature gated opened!")]
    [InlineData(false, "Feature gated closed.")]
    public async Task When_UsingSimpleConstructorThenInvoked_Expect_Invoked(bool isOpened, string expected)
    {
        // Arrange
        using SpyActivityListener activityListener = new SpyActivityListener();
        using SpyMeterListener meterListener = new SpyMeterListener();

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task<string> WhenOpened() => Task.FromResult("Feature gated opened!");
        Task<string> WhenClosed() => Task.FromResult("Feature gated closed.");

        // Act
        string result = await new FeatureGateAsync<string>("myFeatureGateKey", ControlledBy, WhenOpened, WhenClosed).InvokeAsync();

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

        Task<bool> ControlledBy() => Task.FromResult(isOpened);
        Task<string> WhenOpened() => Task.FromResult("Feature gated opened!");
        Task<string> WhenClosed() => Task.FromResult("Feature gated closed.");

        // Act
        string result = await new FeatureGateAsync<string>("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed).InvokeAsync();

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(activityListener.Activities);
        Assert.Empty(meterListener.Measurements);
    }
}
