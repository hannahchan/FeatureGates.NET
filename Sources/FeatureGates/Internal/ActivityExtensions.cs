namespace FeatureGates.Internal;

using System;
using System.Diagnostics;

internal static class ActivityExtensions
{
    public static void RecordException(this Activity? activity, Exception exception)
    {
        ActivityTagsCollection tags = new ActivityTagsCollection
        {
            { TraceSemanticConventions.AttributeExceptionType, exception.GetType().FullName },
            { TraceSemanticConventions.AttributeExceptionStacktrace, exception.ToString() },
        };

        if (!string.IsNullOrWhiteSpace(exception.Message))
        {
            tags.Add(TraceSemanticConventions.AttributeExceptionMessage, exception.Message);
        }

        activity?.AddEvent(new ActivityEvent(TraceSemanticConventions.AttributeExceptionEventName, default, tags));
    }

    private static class TraceSemanticConventions
    {
        public const string AttributeExceptionEventName = "exception";

        public const string AttributeExceptionType = "exception.type";

        public const string AttributeExceptionMessage = "exception.message";

        public const string AttributeExceptionStacktrace = "exception.stacktrace";
    }
}
