namespace FeatureGates.UnitTests.Static;

using System;
using System.Threading.Tasks;
using FeatureGates.Static;
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
}
