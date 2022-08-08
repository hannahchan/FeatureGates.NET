namespace FeatureGates.UnitTests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateAsyncUnitTests
{
    [Collection(TestCollection.FeatureGateInvocations)]
    public class Invoke
    {
        [Theory]
        [InlineData(InstrumentType.Counter, true, "Feature gate was opened!")]
        [InlineData(InstrumentType.Counter, false, "Feature gate was closed.")]
        [InlineData(InstrumentType.Histogram, true, "Feature gate was opened!")]
        [InlineData(InstrumentType.Histogram, false, "Feature gate was closed.")]
        public async Task When_FeatureGateInvokedWithFunction_Expect_Function(InstrumentType instrumentType, bool isOpened, string expected)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;

            // Act
            await new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: async () => await Task.FromResult(isOpened),
                whenOpened: async () => await Task.Run(() => result = "Feature gate was opened!"),
                whenClosed: async () => await Task.Run(() => result = "Feature gate was closed."))
                .Invoke();

            // Assert
            Assert.Equal(expected, result);

            Assert.Collection(
                activityListener.Activities,
                activity =>
                {
                    Assert.Equal("FeatureGate", activity.OperationName);

                    Assert.Collection(
                        activity.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    switch (instrumentType)
                    {
                        case InstrumentType.Counter:
                            Assert.Equal("feature.gate.executions", measurement.Instrument.Name);
                            Assert.Null(measurement.Instrument.Unit);
                            Assert.Equal("measures the number of times a feature gate has been executed", measurement.Instrument.Description);
                            Assert.Equal(1, Assert.IsType<int>(measurement.Value));
                            break;

                        case InstrumentType.Histogram:
                            Assert.Equal("feature.gate.duration", measurement.Instrument.Name);
                            Assert.Equal("ms", measurement.Instrument.Unit);
                            Assert.Equal("measures the duration of feature gate executions", measurement.Instrument.Description);
                            Assert.IsType<double>(measurement.Value);
                            break;
                    }

                    Assert.Collection(
                        measurement.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(InstrumentType.Counter, true)]
        [InlineData(InstrumentType.Counter, false)]
        [InlineData(InstrumentType.Histogram, true)]
        [InlineData(InstrumentType.Histogram, false)]
        public async Task When_FeatureGateInvokedWithNullFunction_Expect_Null(InstrumentType instrumentType, bool isOpened)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            Exception result;

            // Act
            result = await Record.ExceptionAsync(() => new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: async () => await Task.FromResult(isOpened),
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            // Assert
            Assert.Null(result);

            Assert.Collection(
                activityListener.Activities,
                activity =>
                {
                    Assert.Equal("FeatureGate", activity.OperationName);

                    Assert.Collection(
                        activity.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    switch (instrumentType)
                    {
                        case InstrumentType.Counter:
                            Assert.Equal("feature.gate.executions", measurement.Instrument.Name);
                            Assert.Null(measurement.Instrument.Unit);
                            Assert.Equal("measures the number of times a feature gate has been executed", measurement.Instrument.Description);
                            Assert.Equal(1, Assert.IsType<int>(measurement.Value));
                            break;

                        case InstrumentType.Histogram:
                            Assert.Equal("feature.gate.duration", measurement.Instrument.Name);
                            Assert.Equal("ms", measurement.Instrument.Unit);
                            Assert.Equal("measures the duration of feature gate executions", measurement.Instrument.Description);
                            Assert.IsType<double>(measurement.Value);
                            break;
                    }

                    Assert.Collection(
                        measurement.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(InstrumentType.Counter, true, "Opened gate threw an exception.")]
        [InlineData(InstrumentType.Counter, false, "Closed gate threw an exception.")]
        [InlineData(InstrumentType.Histogram, true, "Opened gate threw an exception.")]
        [InlineData(InstrumentType.Histogram, false, "Closed gate threw an exception.")]
        public async Task When_FeatureGateInvokedWithFunctionThrowingException_Expect_Exception(InstrumentType instrumentType, bool isOpened, string expected)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            Exception result;

            // Act
            result = await Record.ExceptionAsync(() => new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: async () => await Task.FromResult(isOpened),
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Message);

            Assert.Collection(
                activityListener.Activities,
                activity =>
                {
                    Assert.Equal("FeatureGate", activity.OperationName);

                    Assert.Collection(
                        activity.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Error, activity.Status);
                    Assert.Equal("An uncaught exception occurred during feature gate execution.", activity.StatusDescription);
                    Assert.Collection(activity.Events, @event => Assert.Equal("exception", @event.Name));
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    switch (instrumentType)
                    {
                        case InstrumentType.Counter:
                            Assert.Equal("feature.gate.executions", measurement.Instrument.Name);
                            Assert.Null(measurement.Instrument.Unit);
                            Assert.Equal("measures the number of times a feature gate has been executed", measurement.Instrument.Description);
                            Assert.Equal(1, Assert.IsType<int>(measurement.Value));
                            break;

                        case InstrumentType.Histogram:
                            Assert.Equal("feature.gate.duration", measurement.Instrument.Name);
                            Assert.Equal("ms", measurement.Instrument.Unit);
                            Assert.Equal("measures the duration of feature gate executions", measurement.Instrument.Description);
                            Assert.IsType<double>(measurement.Value);
                            break;
                    }

                    Assert.Collection(
                        measurement.Tags,
                        tag =>
                        {
                            Assert.Equal("feature.gate.key", tag.Key);
                            Assert.Equal("myFeatureGateKey", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.state", tag.Key);
                            Assert.Equal(isOpened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("true", tag.Value);
                        });
                });
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class WhenOpened
    {
        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewFeatureGateWithNewFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => whenOpened = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(() => whenOpened = "Updated Action.");
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenNullAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(null as Action);
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal(string.Empty, whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class WhenClosed
    {
        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTask_Expect_NewFeatureGateWithNewFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => whenClosed = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(() => whenClosed = "Updated Action.");
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Action.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedNullAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGateAsync featureGate = new FeatureGateAsync(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => whenOpened = "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => whenClosed = "Feature gate was closed."));

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(null as Action);
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal(string.Empty, whenClosed);
        }
    }
}
