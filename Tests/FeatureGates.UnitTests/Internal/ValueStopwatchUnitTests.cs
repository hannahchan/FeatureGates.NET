namespace FeatureGates.UnitTests.Internal;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;
using Xunit;

public class ValueStopwatchUnitTests
{
    [Fact]
    public void When_GettingElapsedTime_Expect_InvalidOperationException()
    {
        // Arrange
        ValueStopwatch stopwatch = default;

        // Act
        Exception exception = Record.Exception(() => stopwatch.GetElapsedTime());

        // Assert
        InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.", invalidOperationException.Message);
    }

    [Fact]
    public async Task When_GettingElapsedTime_Expect_ElapsedTime()
    {
        // Arrange
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        // Act
        await Task.Delay(100);
        TimeSpan elapsedTime1 = stopwatch.GetElapsedTime();

        await Task.Delay(100);
        TimeSpan elapsedTime2 = stopwatch.GetElapsedTime();

        await Task.Delay(100);
        TimeSpan elapsedTime3 = stopwatch.GetElapsedTime();

        // Assert
        Assert.True(elapsedTime1.TotalMilliseconds > 100);
        Assert.True(elapsedTime1.TotalMilliseconds < 200);

        Assert.True(elapsedTime2.TotalMilliseconds > 200);
        Assert.True(elapsedTime2.TotalMilliseconds < 300);

        Assert.True(elapsedTime3.TotalMilliseconds > 300);
        Assert.True(elapsedTime3.TotalMilliseconds < 400);

        Assert.True(elapsedTime1.Ticks < elapsedTime2.Ticks);
        Assert.True(elapsedTime2.Ticks < elapsedTime3.Ticks);
    }
}
