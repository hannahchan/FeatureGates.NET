namespace FeatureGates.Internal;

using System;
using System.Diagnostics;

internal struct ValueStopwatch
{
    private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    private readonly long startTimestamp;

    private ValueStopwatch(long startTimestamp)
    {
        this.startTimestamp = startTimestamp;
    }

    public static ValueStopwatch StartNew()
    {
        return new ValueStopwatch(Stopwatch.GetTimestamp());
    }

    public TimeSpan GetElapsedTime()
    {
        // Start timestamp can't be zero in an initialized ValueStopwatch.
        // It would have to be literally the first thing executed when the machine boots to be 0.
        // So it being 0 is a clear indication of default(ValueStopwatch).
        if (this.startTimestamp == 0)
        {
            throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
        }

        long endTimestamp = Stopwatch.GetTimestamp();
        long deltaTimestamp = endTimestamp - this.startTimestamp;

        long ticks = (long)(TimestampToTicks * deltaTimestamp);

        return new TimeSpan(ticks);
    }
}
