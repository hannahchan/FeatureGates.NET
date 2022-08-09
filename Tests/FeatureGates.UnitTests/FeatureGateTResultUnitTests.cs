namespace FeatureGates.UnitTests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateTResultUnitTests
{
    [Collection(TestCollection.FeatureGateInvocations)]
    public class Invoke
    {
        [Theory]
        [InlineData(InstrumentType.Counter, true, "Feature gate was opened!")]
        [InlineData(InstrumentType.Counter, false, "Feature gate was closed.")]
        [InlineData(InstrumentType.Histogram, true, "Feature gate was opened!")]
        [InlineData(InstrumentType.Histogram, false, "Feature gate was closed.")]
        public void When_FeatureGateInvokedWithFunction_Expect_Function(InstrumentType instrumentType, bool isOpened, string expected)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;

            // Act
            result = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.")
                .Invoke();

            // Assert
            Assert.Equal(expected, result);

            Assert.Collection(
                activityListener.Activities,
                activity =>
                {
                    Assert.Equal("feature.gate.execution", activity.OperationName);

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
        public void When_FeatureGateInvokedWithNullFunction_Expect_Null(InstrumentType instrumentType, bool isOpened)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;
            Exception exception;

            // Act
            exception = Record.Exception(() => result = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            // Assert
            Assert.Null(result);
            Assert.Null(exception);

            Assert.Collection(
                activityListener.Activities,
                activity =>
                {
                    Assert.Equal("feature.gate.execution", activity.OperationName);

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
        public void When_FeatureGateInvokedWithFunctionThrowingException_Expect_Exception(InstrumentType instrumentType, bool isOpened, string expected)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            Exception result;

            // Act
            result = Record.Exception(() => new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                instrumentType: instrumentType,
                controlledBy: () => isOpened,
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
                    Assert.Equal("feature.gate.execution", activity.OperationName);

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
        public void When_FeatureGateInvokedWithWhenOpenAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGate<string> newFeatureGate = featureGate.WhenOpened(() => "Updated Action.");
            string whenOpened = newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTaskAndWhenClosedNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: null);

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Null(whenClosed);
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class WhenClosed
    {
        [Fact]
        public void When_FeatureGateInvokedWithWhenClosedAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGate<string> newFeatureGate = featureGate.WhenClosed(() => "Updated Action.");
            string whenOpened = newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Action.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTask_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTaskAndWhenOpenedNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Null(whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }
    }
}
