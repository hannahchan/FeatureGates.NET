namespace FeatureGates.UnitTests;

using System;
using System.Threading.Tasks;
using Xunit;

public class FeatureGateAsyncUnitTests
{
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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

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
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal(string.Empty, whenClosed);
        }
    }
}
