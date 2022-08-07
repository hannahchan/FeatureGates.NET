#pragma warning disable CS4014

namespace ExampleApp;

using System;
using System.Threading;
using System.Threading.Tasks;
using FeatureGates;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal static class Program
{
    private static void Main()
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService("ExampleApp");

        using MeterProvider meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddFeatureGates()
            .AddPrometheusExporter(options =>
            {
                options.StartHttpListener = true;
                options.HttpListenerPrefixes = new string[] { $"http://localhost:9464/" };
                options.ScrapeEndpointPath = "/metrics";
                options.ScrapeResponseCacheDurationMilliseconds = 0;
            })
            .Build();

        using TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddFeatureGates()
            .Build();

        FeatureGateAsync featureGate1 = FeatureGate
            .WithKey("MyCounterGate")
            .ControlledBy(Example.BooleanFunction)
            .WhenOpened(Example.ActionAsync)
            .WhenClosed(Example.ActionAsync);

        FeatureGateAsync featureGate2 = FeatureGate
            .WithKey("MyHistogramGate")
            .WithHistogram()
            .ControlledBy(Example.BooleanFunction)
            .WhenOpened(Example.ActionAsync)
            .WhenClosed(Example.ActionAsync);

        Console.WriteLine("View metrics at: http://localhost:9464/metrics");
        Console.WriteLine("Press any key to exit");

        while (!Console.KeyAvailable)
        {
            Thread.Sleep(200);

            // Do not await.
            featureGate1.Invoke();
            featureGate2.Invoke();
        }
    }

    private static class Example
    {
        private static readonly Random Random = new Random();

        public static bool BooleanFunction()
        {
            // 50% chance for true or false
            return Random.NextDouble() >= 0.5;
        }

        public static async Task ActionAsync()
        {
            // 25% chance to complete in over 300 ms
            // 75% chance to complete in under 300 ms
            if (Random.NextDouble() >= 0.75)
            {
                await Task.Delay(Random.Next(300, 1000));
            }
            else
            {
                await Task.Delay(Random.Next(0, 300));
            }

            // 10% chance to throw exception.
            if (Random.NextDouble() >= 0.9)
            {
                throw new InvalidOperationException("This is an example exception.");
            }
        }
    }
}
