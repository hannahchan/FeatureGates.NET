namespace FeatureGates.UnitTests.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class ActivityListenerSpy : IDisposable
{
    private readonly ActivityListener activityListener = new ActivityListener
    {
        ShouldListenTo = source => true,
        SampleUsingParentId = (ref ActivityCreationOptions<string> options) => ActivitySamplingResult.AllData,
        Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
    };

    private bool disposed;

    public ActivityListenerSpy()
    {
        this.activityListener.ActivityStarted = this.ActivityStarted;
        ActivitySource.AddActivityListener(this.activityListener);
    }

    public List<Activity> Activities { get; } = new List<Activity>();

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.activityListener.Dispose();
    }

    public void ActivityStarted(Activity activity)
    {
        if (this.disposed)
        {
            return;
        }

        this.Activities.Add(activity);
    }
}
