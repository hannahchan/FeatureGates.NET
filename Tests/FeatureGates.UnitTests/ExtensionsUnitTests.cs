namespace FeatureGates.UnitTests;

using System;
using System.Collections.Generic;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Xunit;

public class ExtensionsUnitTests
{
    [Fact]
    public void When_AddingFeatureGatesToMeterProviderBuilder_Expect_FeatureGatesAdded()
    {
        // Arrange
        MockMeterProviderBuilder builder = new MockMeterProviderBuilder();

        // Act
        builder.AddFeatureGates();

        // Assert
        Assert.Collection(builder.Meters, meter => Assert.Equal("FeatureGates", meter));
    }

    [Fact]
    public void When_AddingFeatureGatesToTracerProviderBuilder_Expect_FeatureGatesAdded()
    {
        // Arrange
        MockTracerProviderBuilder builder = new MockTracerProviderBuilder();

        // Act
        builder.AddFeatureGates();

        // Assert
        Assert.Collection(builder.Sources, meter => Assert.Equal("FeatureGates", meter));
    }

    private class MockMeterProviderBuilder : MeterProviderBuilder
    {
        public List<string> Meters { get; } = new List<string>();

        public override MeterProviderBuilder AddInstrumentation<TInstrumentation>(Func<TInstrumentation> instrumentationFactory)
        {
            throw new NotImplementedException();
        }

        public override MeterProviderBuilder AddMeter(params string[] names)
        {
            this.Meters.AddRange(names);
            return this;
        }
    }

    private class MockTracerProviderBuilder : TracerProviderBuilder
    {
        public List<string> Sources { get; } = new List<string>();

        public override TracerProviderBuilder AddInstrumentation<TInstrumentation>(Func<TInstrumentation> instrumentationFactory)
        {
            throw new NotImplementedException();
        }

        public override TracerProviderBuilder AddLegacySource(string operationName)
        {
            throw new NotImplementedException();
        }

        public override TracerProviderBuilder AddSource(params string[] names)
        {
            this.Sources.AddRange(names);
            return this;
        }
    }
}
