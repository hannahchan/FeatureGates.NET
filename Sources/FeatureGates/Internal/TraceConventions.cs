namespace FeatureGates.Internal;

internal static class TraceConventions
{
    public const string AttributeFeatureGateKey = "feature.gate.key";

    public const string AttributeFeatureGateState = "feature.gate.state";

    // Trace Semantic Conventions for Exceptions are from:
    // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md
    public const string AttributeExceptionEventName = "exception";

    public const string AttributeExceptionType = "exception.type";

    public const string AttributeExceptionMessage = "exception.message";

    public const string AttributeExceptionStacktrace = "exception.stacktrace";
}
