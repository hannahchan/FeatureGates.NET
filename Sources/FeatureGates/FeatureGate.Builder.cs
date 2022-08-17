namespace FeatureGates;

using System;
using System.Threading.Tasks;

public partial class FeatureGate
{
    public static Builder WithKey(string featureGateKey)
    {
        return new Builder(featureGateKey);
    }

    public class Builder
    {
        internal Builder(string featureGateKey)
        {
            this.Key = featureGateKey;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; private set; } = InstrumentType.Counter;

        public bool FallbackOnException { get; private set; }

        public Builder WithHistogram()
        {
            this.InstrumentType = InstrumentType.Histogram;
            return this;
        }

        public Builder WithFallbackOnException()
        {
            this.FallbackOnException = true;
            return this;
        }

        public BaseGate ControlledBy(Func<bool> predicate)
        {
            return new BaseGate(this.Key, this.InstrumentType, this.FallbackOnException, predicate);
        }

        public BaseGateAsync ControlledBy(Func<Task<bool>> predicate)
        {
            return new BaseGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, predicate);
        }
    }

    public class BaseGate : AbstractFeatureGate
    {
        internal BaseGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
        }

        public Func<bool> ControlledBy { get; }

        public HalfGate WhenOpened(Action? action)
        {
            return new HalfGate(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, action);
        }

        public PartialResultGate<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return new PartialResultGate<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
        }

        public HalfGateAsync WhenOpened(Func<Task> function)
        {
            return new HalfGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, () => Task.Run(this.ControlledBy), function);
        }

        public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialResultGateAsync<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, () => Task.Run(this.ControlledBy), function);
        }
    }

    public class BaseGateAsync : AbstractFeatureGate
    {
        internal BaseGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
        }

        public Func<Task<bool>> ControlledBy { get; }

        public HalfGateAsync WhenOpened(Func<Task> function)
        {
            return new HalfGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
        }

        public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialResultGateAsync<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
        }

        public HalfGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }

    public class HalfGate : FeatureGate
    {
        internal HalfGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
        {
        }

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

    public class HalfGateAsync : FeatureGateAsync
    {
        internal HalfGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
        {
        }

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

    public class PartialResultGate<TResult> : AbstractFeatureGate
    {
        internal PartialResultGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public Func<bool> ControlledBy { get; }

        public Func<TResult> WhenOpened { get; }

        public FeatureGate<TResult> WhenClosed(Func<TResult> function)
        {
            return new FeatureGate<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: () => Task.Run(this.ControlledBy),
                whenOpened: () => Task.Run(this.WhenOpened),
                whenClosed: function);
        }
    }

    public class PartialResultGateAsync<TResult> : AbstractFeatureGate
    {
        internal PartialResultGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public Func<Task<bool>> ControlledBy { get; }

        public Func<Task<TResult>> WhenOpened { get; }

        public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        public FeatureGateAsync<TResult> WhenClosed(Func<TResult> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: () => Task.Run(function));
        }
    }
}
