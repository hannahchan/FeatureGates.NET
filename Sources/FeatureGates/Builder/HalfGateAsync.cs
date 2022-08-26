namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents an asynchronous half gate, an invocable feature gate with a <c>null</c> 'WhenClosed' delegate. Create a full gate by specifying a 'WhenClosed' delegate.</summary>
public class HalfGateAsync : FeatureGateAsync
{
    internal HalfGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened)
        : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
    {
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
            controlledBy: this.ControlledBy,
            whenOpened: this.WhenOpened,
            whenClosed: function);
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
    /// <param name="action">The delegate expressed as an action.</param>
    /// <returns>A completed <see cref="FeatureGateAsync" />.</returns>
    public new FeatureGateAsync WhenClosed(Action? action)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            fallbackOnException: this.FallbackOnException,
            controlledBy: this.ControlledBy,
            whenOpened: this.WhenOpened,
            whenClosed: action == null ? null : () => Task.Run(action));
    }
}
