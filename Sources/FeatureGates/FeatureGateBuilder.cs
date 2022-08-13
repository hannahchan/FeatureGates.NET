namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateBuilder
{
    public FeatureGateBuilder(string featureGateKey)
    {
        this.Key = featureGateKey;
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; private set; } = InstrumentType.Counter;

    public FeatureGateBuilder WithHistogram()
    {
        this.InstrumentType = InstrumentType.Histogram;
        return this;
    }

    public BaseFeatureGate ControlledBy(Func<bool> predicate)
    {
        return new BaseFeatureGate(this.Key, this.InstrumentType, predicate);
    }

    public BaseFeatureGateAsync ControlledBy(Func<Task<bool>> predicate)
    {
        return new BaseFeatureGateAsync(this.Key, this.InstrumentType, predicate);
    }

    public class BaseFeatureGate : AbstractFeatureGate
    {
        public BaseFeatureGate(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy)
            : base(featureGateKey, instrumentType)
        {
            this.ControlledBy = controlledBy;
        }

        public Func<bool> ControlledBy { get; }

        public PartialFeatureGate WhenOpened(Action? action)
        {
            return new PartialFeatureGate(this.Key, this.InstrumentType, this.ControlledBy, action);
        }

        public PartialFeatureGate<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return new PartialFeatureGate<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function);
        }

        public PartialFeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new PartialFeatureGateAsync(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function);
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialFeatureGateAsync<TResult>(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function);
        }
    }

    public class BaseFeatureGateAsync : AbstractFeatureGate
    {
        public BaseFeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy)
            : base(featureGateKey, instrumentType)
        {
            this.ControlledBy = controlledBy;
        }

        public Func<Task<bool>> ControlledBy { get; }

        public PartialFeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new PartialFeatureGateAsync(this.Key, this.InstrumentType, this.ControlledBy, function);
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialFeatureGateAsync<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function);
        }

        public PartialFeatureGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }

    public class PartialFeatureGate : AbstractFeatureGate
    {
        public PartialFeatureGate(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Action? whenOpened)
            : base(featureGateKey, instrumentType)
        {
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public Func<bool> ControlledBy { get; }

        public Action? WhenOpened { get; }

        public FeatureGate WhenClosed(Action? action)
        {
            return new FeatureGate(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: action);
        }

        public FeatureGateAsync WhenClosed(Func<Task>? function)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: () => Task.Run(this.ControlledBy),
                whenOpened: this.WhenOpened == null ? null : () => Task.Run(this.WhenOpened),
                whenClosed: function);
        }

        public void Invoke()
        {
            StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.ControlledBy, this.WhenOpened, null);
        }
    }

    public class PartialFeatureGateAsync : AbstractFeatureGate
    {
        public PartialFeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task>? whenOpened)
            : base(featureGateKey, instrumentType)
        {
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public Func<Task<bool>> ControlledBy { get; }

        public Func<Task>? WhenOpened { get; }

        public FeatureGateAsync WhenClosed(Func<Task>? function)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        public FeatureGateAsync WhenClosed(Action? action)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: action == null ? null : () => Task.Run(action));
        }

        public async Task InvokeAsync()
        {
            await StaticFeatureGate.InvokeAsync(this.Key, this.InstrumentType, this.ControlledBy, this.WhenOpened, null);
        }
    }

    public class PartialFeatureGate<TResult> : AbstractFeatureGate
    {
        public PartialFeatureGate(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Func<TResult> whenOpened)
            : base(featureGateKey, instrumentType)
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
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: () => Task.Run(this.ControlledBy),
                whenOpened: () => Task.Run(this.WhenOpened),
                whenClosed: function);
        }
    }

    public class PartialFeatureGateAsync<TResult> : AbstractFeatureGate
    {
        public PartialFeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened)
            : base(featureGateKey, instrumentType)
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
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        public FeatureGateAsync<TResult> WhenClosed(Func<TResult> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: () => Task.Run(function));
        }
    }
}
