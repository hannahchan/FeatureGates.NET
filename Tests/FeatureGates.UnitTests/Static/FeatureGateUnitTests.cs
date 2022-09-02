namespace FeatureGates.UnitTests.Static;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FeatureGates.Static;
using FeatureGates.UnitTests.Helpers;
using Xunit;

public class FeatureGateUnitTests
{
    public class Invoke
    {
        [Theory]
        [InlineData(true, "Feature gate opened!")]
        [InlineData(false, "Feature gate closed.")]
        public void When_Invoked_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            bool ControlledBy() => isOpened;
            void WhenOpened() => result = "Feature gate opened!";
            void WhenClosed() => result = "Feature gate closed.";

            // Act
            FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, false, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void When_InvokedWithFallback_Expect_WhenClosedInvoked()
        {
            // Arrange
            string result = string.Empty;

            bool ControlledBy() => true;
            void WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            void WhenClosed() => result = "Feature gate closed.";

            // Act
            FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal("Feature gate closed.", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void When_InvokedWithFallback_Expect_Exception(bool fallbackOnException)
        {
            // Arrange
            bool ControlledBy() => true;
            void WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            void WhenClosed() => throw new InvalidOperationException("Feature gate closed.");

            // Act
            Exception exception = Record.Exception(() => FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, fallbackOnException, ControlledBy, WhenOpened, WhenClosed));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(fallbackOnException ? "Feature gate closed." : "Feature gate opened!", exception.Message);
        }
    }

    public class InvokeTResult
    {
        [Theory]
        [InlineData(true, "Feature gate opened!")]
        [InlineData(false, "Feature gate closed.")]
        public void When_Invoked_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            bool ControlledBy() => isOpened;
            string WhenOpened() => "Feature gate opened!";
            string WhenClosed() => "Feature gate closed.";

            // Act
            string result = FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, false, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void When_InvokedWithFallback_Expect_WhenClosedInvoked()
        {
            // Arrange
            bool ControlledBy() => true;
            string WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            string WhenClosed() => "Feature gate closed.";

            // Act
            string result = FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal("Feature gate closed.", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void When_InvokedWithFallback_Expect_Exception(bool fallbackOnException)
        {
            // Arrange
            bool ControlledBy() => true;
            string WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            string WhenClosed() => throw new InvalidOperationException("Feature gate closed.");

            // Act
            Exception exception = Record.Exception(() => FeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, fallbackOnException, ControlledBy, WhenOpened, WhenClosed));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(fallbackOnException ? "Feature gate closed." : "Feature gate opened!", exception.Message);
        }
    }

    public class InvokeAsync
    {
        [Theory]
        [InlineData(true, "Feature gate opened!")]
        [InlineData(false, "Feature gate closed.")]
        public async Task When_InvokedAsync_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            Task<bool> ControlledBy() => Task.FromResult(isOpened);
            Task WhenOpened() => Task.Run(() => result = "Feature gate opened!");
            Task WhenClosed() => Task.Run(() => result = "Feature gate closed.");

            // Act
            await FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, false, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task When_InvokedAsyncWithFallback_Expect_WhenClosedInvoked()
        {
            // Arrange
            string result = string.Empty;

            Task<bool> ControlledBy() => Task.FromResult(true);
            Task WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            Task WhenClosed() => Task.Run(() => result = "Feature gate closed.");

            // Act
            await FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal("Feature gate closed.", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task When_InvokedAsyncWithFallback_Expect_Exception(bool fallbackOnException)
        {
            // Arrange
            Task<bool> ControlledBy() => Task.FromResult(true);
            Task WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            Task WhenClosed() => throw new InvalidOperationException("Feature gate closed.");

            // Act
            Exception exception = await Record.ExceptionAsync(() => FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, fallbackOnException, ControlledBy, WhenOpened, WhenClosed));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(fallbackOnException ? "Feature gate closed." : "Feature gate opened!", exception.Message);
        }
    }

    public class InvokeAsyncTResult
    {
        [Theory]
        [InlineData(true, "Feature gate opened!")]
        [InlineData(false, "Feature gate closed.")]
        public async Task When_InvokedAsync_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            Task<bool> ControlledBy() => Task.FromResult(isOpened);
            Task<string> WhenOpened() => Task.FromResult("Feature gate opened!");
            Task<string> WhenClosed() => Task.FromResult("Feature gate closed.");

            // Act
            string result = await FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, false, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task When_InvokedAsyncWithFallback_Expect_WhenClosedInvoked()
        {
            // Arrange
            Task<bool> ControlledBy() => Task.FromResult(true);
            Task<string> WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            Task<string> WhenClosed() => Task.FromResult("Feature gate closed.");

            // Act
            string result = await FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, true, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal("Feature gate closed.", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task When_InvokedAsyncWithFallback_Expect_Exception(bool fallbackOnException)
        {
            // Arrange
            Task<bool> ControlledBy() => Task.FromResult(true);
            Task<string> WhenOpened() => throw new InvalidOperationException("Feature gate opened!");
            Task<string> WhenClosed() => throw new InvalidOperationException("Feature gate closed.");

            // Act
            Exception exception = await Record.ExceptionAsync(() => FeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, fallbackOnException, ControlledBy, WhenOpened, WhenClosed));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(fallbackOnException ? "Feature gate closed." : "Feature gate opened!", exception.Message);
        }
    }

    public class RecordExecution
    {
        [Fact]
        public void When_RecordingExecution_Expected_Recorded()
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;
            void Action() => result = "Action executed!";

            // Act
            FeatureGate.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                instrumentType: InstrumentType.Counter,
                action: Action);

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
                            Assert.Equal("opened", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    Assert.Equal(1, Assert.IsType<long>(measurement.Value));

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
                            Assert.Equal("opened", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Fact]
        public void When_RecordingExecutionReturningResult_Expected_Recorded()
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = FeatureGate.RecordExecution(
                featureGateKey: "myFeatureGateKey",
                instrumentType: InstrumentType.Counter,
                function: () => "Function executed!");

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
                            Assert.Equal("opened", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    Assert.Equal(1, Assert.IsType<long>(measurement.Value));

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
                            Assert.Equal("opened", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }
    }

    public class RecordExecutionAsync
    {
        [Fact]
        public async Task When_RecordingExecution_Expected_Recorded()
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            string result = string.Empty;
            Task Function() => Task.Run(() => result = "Function executed!");

            // Act
            await FeatureGate.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                instrumentType: InstrumentType.Counter,
                function: Function);

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
                            Assert.Equal("opened", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    Assert.Equal(1, Assert.IsType<long>(measurement.Value));

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
                            Assert.Equal("opened", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }

        [Fact]
        public async Task When_RecordingExecutionReturningResult_Expected_Recorded()
        {
            // Arrange
            using SpyActivityListener activityListener = new SpyActivityListener();
            using SpyMeterListener meterListener = new SpyMeterListener();

            // Act
            string result = await FeatureGate.RecordExecutionAsync(
                featureGateKey: "myFeatureGateKey",
                instrumentType: InstrumentType.Counter,
                function: () => Task.FromResult("Function executed!"));

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
                            Assert.Equal("opened", tag.Value);
                        });

                    Assert.Equal(ActivityStatusCode.Ok, activity.Status);
                    Assert.Null(activity.StatusDescription);
                });

            Assert.Collection(
                meterListener.Measurements,
                measurement =>
                {
                    Assert.Equal(1, Assert.IsType<long>(measurement.Value));

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
                            Assert.Equal("opened", tag.Value);
                        },
                        tag =>
                        {
                            Assert.Equal("feature.gate.exception", tag.Key);
                            Assert.Equal("false", tag.Value);
                        });
                });
        }
    }
}
