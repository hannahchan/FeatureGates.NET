namespace FeatureGates.UnitTests;

using System.Threading.Tasks;
using Xunit;

public class FeatureGateAsyncTResultUnitTests
{
    [Collection(TestCollection.FeatureGateInvocations)]
    public class WhenOpened
    {
        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewFeatureGateWithNewFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => "Updated Action.");
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
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

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => "Updated Action.");
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Action.", whenClosed);
        }
    }
}
