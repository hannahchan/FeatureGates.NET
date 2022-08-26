namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents a half gate, an invocable feature gate with a <c>null</c> 'WhenClosed' delegate. Create a full gate by specifying a 'WhenClosed' delegate.</summary>
public class HalfGate : FeatureGate
{
    internal HalfGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened)
        : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
    {
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
    /// <param name="action">The delegate expressed as an action.</param>
    /// <returns>A completed <see cref="FeatureGate" />.</returns>
    public new FeatureGate WhenClosed(Action? action)
    {
        return new FeatureGate(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            fallbackOnException: this.FallbackOnException,
            controlledBy: this.ControlledBy,
            whenOpened: this.WhenOpened,
            whenClosed: action);
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
    /// <param name="function">The delegate expressed as an asynchronous action.</param>
    /// <returns>A completed <see cref="FeatureGateAsync" />.</returns>
    public new FeatureGateAsync WhenClosed(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            fallbackOnException: this.FallbackOnException,
            controlledBy: () => Task.Run(this.ControlledBy),
            whenOpened: this.WhenOpened == null ? null : () => Task.Run(this.WhenOpened),
            whenClosed: function);
    }
}
