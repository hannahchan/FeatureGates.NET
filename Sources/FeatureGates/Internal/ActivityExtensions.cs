namespace FeatureGates.Internal;

using System;
using System.Diagnostics;

internal static class ActivityExtensions
{
    public static string? GetSpanId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            _ => null,
        };
    }

    public static string? GetTraceId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            _ => null,
        };
    }

    public static string? GetParentId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.ParentId,
            ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
            _ => null,
        };
    }

    public static Activity RecordException(this Activity activity, Exception exception)
    {
        ActivityTagsCollection tagsCollection = new ActivityTagsCollection
        {
            { TraceConventions.AttributeExceptionType, exception.GetType().FullName },
            { TraceConventions.AttributeExceptionMessage, exception.Message },
            { TraceConventions.AttributeExceptionStacktrace, exception.ToString() },
        };

        return activity.AddEvent(new ActivityEvent(TraceConventions.AttributeExceptionEventName, default, tagsCollection));
    }
}
