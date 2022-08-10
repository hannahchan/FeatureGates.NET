namespace FeatureGates.UnitTests.Internal;

using System;
using System.Diagnostics;
using FeatureGates.Internal;
using Xunit;

public class ActivityExtensionsUnitTests
{
    [Fact]
    public void When_RecordingException_Expect_ActivityEvent()
    {
        // Arrange
        using Activity activity = new Activity(nameof(activity));
        InvalidOperationException exception = new InvalidOperationException("My test exception.");

        // Act
        activity.RecordException(exception);

        // Assert
        Assert.Collection(
            activity.Events,
            @event =>
            {
                Assert.Equal("exception", @event.Name);
                Assert.Collection(
                    @event.Tags,
                    tag =>
                    {
                        Assert.Equal("exception.type", tag.Key);
                        Assert.Equal("System.InvalidOperationException", tag.Value);
                    },
                    tag =>
                    {
                        Assert.Equal("exception.stacktrace", tag.Key);
                        Assert.Equal("System.InvalidOperationException: My test exception.", tag.Value);
                    },
                    tag =>
                    {
                        Assert.Equal("exception.message", tag.Key);
                        Assert.Equal("My test exception.", tag.Value);
                    });
            });
    }

    [Fact]
    public void When_RecordingExceptionWithEmptyMessage_Expect_ActivityEvent()
    {
        // Arrange
        using Activity activity = new Activity(nameof(activity));
        InvalidOperationException exception = new InvalidOperationException(string.Empty);

        // Act
        activity.RecordException(exception);

        // Assert
        Assert.Collection(
            activity.Events,
            @event =>
            {
                Assert.Equal("exception", @event.Name);
                Assert.Collection(
                    @event.Tags,
                    tag =>
                    {
                        Assert.Equal("exception.type", tag.Key);
                        Assert.Equal("System.InvalidOperationException", tag.Value);
                    },
                    tag =>
                    {
                        Assert.Equal("exception.stacktrace", tag.Key);
                        Assert.Equal("System.InvalidOperationException", tag.Value);
                    });
            });
    }
}
