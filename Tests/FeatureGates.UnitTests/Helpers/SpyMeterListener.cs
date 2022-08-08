namespace FeatureGates.UnitTests.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

public sealed class SpyMeterListener : IDisposable
{
    private readonly MeterListener meterListener = new MeterListener()
    {
        InstrumentPublished = (instrument, listener) => listener.EnableMeasurementEvents(instrument),
    };

    private bool disposed;

    public SpyMeterListener()
    {
        this.meterListener.SetMeasurementEventCallback<byte>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<short>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<int>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<long>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<float>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<double>(this.OnMeasurementRecorded);
        this.meterListener.SetMeasurementEventCallback<decimal>(this.OnMeasurementRecorded);
        this.meterListener.Start();
    }

    public List<Measurement> Measurements { get; } = new List<Measurement>();

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.meterListener.Dispose();
    }

    public void OnMeasurementRecorded<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object>> tags, object state)
    {
        if (this.disposed)
        {
            return;
        }

        this.Measurements.Add(new Measurement(instrument, measurement, tags.ToArray(), state));
    }

    public record Measurement(Instrument Instrument, object Value, KeyValuePair<string, object>[] Tags, object State);
}
