namespace FeatureGates.UnitTests;
using System.Threading.Tasks;
using Xunit;

public class FeatureGateTResultUnitTests
{
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
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
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
            string whenOpened = await newFeatureGate.InvokeAsync();

            isOpened = false;
            string whenClosed = await newFeatureGate.InvokeAsync();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }
    }
}
