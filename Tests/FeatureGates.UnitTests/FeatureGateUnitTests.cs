namespace FeatureGates.UnitTests;

using System;
using System.Threading.Tasks;
using Xunit;

public class FeatureGateUnitTests
{
    public class WithKey
    {
        [Fact]
        public void When_InvokingStaticMethodWithKey_Expect_FeatureGateBuilder()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateBuilder builder = FeatureGate.WithKey(featureGateKey);

            // Assert
            Assert.Equal(featureGateKey, builder.Key);
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

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGate newFeatureGate = featureGate.WhenOpened(() => whenOpened = "Updated Action.");
            newFeatureGate.Invoke();

            isOpened = false;
            newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewGateAsyncWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

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
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTaskAndWhenClosedNull_Expect_NewGateAsyncWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: null);

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => whenOpened = "Updated Function."));
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal(string.Empty, whenClosed);
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

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGate newFeatureGate = featureGate.WhenClosed(() => whenClosed = "Updated Action.");
            newFeatureGate.Invoke();

            isOpened = false;
            newFeatureGate.Invoke();

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

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

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
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTaskAndWhenOpenNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => whenClosed = "Updated Function."));
            await newFeatureGate.InvokeAsync();

            isOpened = false;
            await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal(string.Empty, whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }
    }
}
