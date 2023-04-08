namespace FeatureGates.UnitTests.Internal;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FeatureGates.Internal;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class InstrumentationUnitTests
{
    public class RecordExecution
    {
        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal void When_RecordingExecution_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;
            void Action() => result = "Action executed!";

            // Act
            Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                action: Action,
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Action executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal void When_RecordingExecutionWithNullAction_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = Record.Exception(() => Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                action: null,
                instrumentType: instrumentType));

            // Assert
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal void When_RecordingExecutionWithActionThrowingException_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = Record.Exception(() => Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                action: () => throw new InvalidOperationException("An exception was thrown."),
                instrumentType: instrumentType));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal("An exception was thrown.", exception.Message);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("true", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal void When_RecordingExecutionWithNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;
            void Action() => result = "Action executed!";

            // Act
            Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                action: Action,
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Action executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal void When_RecordingExecutionWithWithNullActionAndNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = Record.Exception(() => Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                action: null,
                instrumentType: instrumentType));

            // Assert
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }
    }

    public class RecordExecutionTResult
    {
        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal void When_RecordingExecution_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => "Function executed!",
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal void When_RecordingExecutionWithFunctionThrowingException_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = Record.Exception(() => Instrumentation.RecordExecution<string>(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => throw new InvalidOperationException("An exception was thrown."),
                instrumentType: instrumentType));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal("An exception was thrown.", exception.Message);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("true", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal void When_RecordingExecutionWithNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = Instrumentation.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => "Function executed!",
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }
    }

    public class RecordExecutionAsync
    {
        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal async Task When_RecordingExecution_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            string result = string.Empty;
            Task Function() => Task.Run(() => result = "Function executed!");

            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            await Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: Function,
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal async Task When_RecordingExecutionWithNullFunction_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = await Record.ExceptionAsync(() => Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: null,
                instrumentType: instrumentType));

            // Assert
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal async Task When_RecordingExecutionWithFunctionThrowingException_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = await Record.ExceptionAsync(() => Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => throw new InvalidOperationException("An exception was thrown."),
                instrumentType: instrumentType));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal("An exception was thrown.", exception.Message);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("true", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal async Task When_RecordingExecutionWithNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            string result = string.Empty;
            Task Function() => Task.Run(() => result = "Function executed!");

            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            await Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: Function,
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal async Task When_RecordingExecutionWithWithNullFunctionAndNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = await Record.ExceptionAsync(() => Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: null,
                instrumentType: instrumentType));

            // Assert
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }
    }

    public class RecordExecutionAsyncTResult
    {
        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal async Task When_RecordingExecution_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = await Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => Task.FromResult("Function executed!"),
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Closed, InstrumentType.Histogram)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Counter)]
        [InlineData(FeatureGateState.Opened, InstrumentType.Histogram)]
        internal async Task When_RecordingExecutionWithFunctionThrowingException_Expect_Recorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            Exception exception = await Record.ExceptionAsync(() => Instrumentation.RecordExecutionAsync<string>(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => throw new InvalidOperationException("An exception was thrown."),
                instrumentType: instrumentType));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal("An exception was thrown.", exception.Message);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
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
                            Assert.Equal(1, Assert.IsType<long>(measurement.Value));
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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("true", tag.Value);
                        });
                });
        }

        [Theory]
        [InlineData(FeatureGateState.Closed, InstrumentType.None)]
        [InlineData(FeatureGateState.Opened, InstrumentType.None)]
        internal async Task When_RecordingExecutionWithNoInstrument_Expect_NotRecorded(FeatureGateState featureGateState, InstrumentType instrumentType)
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = await Instrumentation.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                featureGateState: featureGateState,
                function: () => Task.FromResult("Function executed!"),
                instrumentType: instrumentType);

            // Assert
            Assert.Equal("Function executed!", result);

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
                            Assert.Equal(featureGateState == FeatureGateState.Opened ? "opened" : "closed", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Empty(meterListener.Measurements);
        }
    }
}
