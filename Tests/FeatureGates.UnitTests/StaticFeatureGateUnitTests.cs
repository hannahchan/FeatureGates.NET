namespace FeatureGates.UnitTests;

using System.Threading.Tasks;
using Xunit;

public class StaticFeatureGateUnitTests
{
    [Collection(TestCollection.FeatureGateInvocations)]
    public class Invoke
    {
        [Theory]
        [InlineData(true, "Feature gated opened!")]
        [InlineData(false, "Feature gated closed.")]
        public void When_Invoked_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            bool ControlledBy() => isOpened;
            void WhenOpened() => result = "Feature gated opened!";
            void WhenClosed() => result = "Feature gated closed.";

            // Act
            StaticFeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class InvokeTResult
    {
        [Theory]
        [InlineData(true, "Feature gated opened!")]
        [InlineData(false, "Feature gated closed.")]
        public void When_Invoked_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            bool ControlledBy() => isOpened;
            string WhenOpened() => "Feature gated opened!";
            string WhenClosed() => "Feature gated closed.";

            // Act
            string result = StaticFeatureGate.Invoke("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class InvokeAsync
    {
        [Theory]
        [InlineData(true, "Feature gated opened!")]
        [InlineData(false, "Feature gated closed.")]
        public async Task When_InvokedAsync_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            Task<bool> ControlledBy() => Task.FromResult(isOpened);
            Task WhenOpened() => Task.Run(() => result = "Feature gated opened!");
            Task WhenClosed() => Task.Run(() => result = "Feature gated closed.");

            // Act
            await StaticFeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    [Collection(TestCollection.FeatureGateInvocations)]
    public class InvokeAsyncTResult
    {
        [Theory]
        [InlineData(true, "Feature gated opened!")]
        [InlineData(false, "Feature gated closed.")]
        public async Task When_InvokedAsync_Expect_Invoked(bool isOpened, string expected)
        {
            // Arrange
            Task<bool> ControlledBy() => Task.FromResult(isOpened);
            Task<string> WhenOpened() => Task.FromResult("Feature gated opened!");
            Task<string> WhenClosed() => Task.FromResult("Feature gated closed.");

            // Act
            string result = await StaticFeatureGate.InvokeAsync("myFeatureGateKey", InstrumentType.None, ControlledBy, WhenOpened, WhenClosed);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
