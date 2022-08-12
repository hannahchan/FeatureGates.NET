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

    public PartialFeatureGate ControlledBy(Func<bool> predicate)
    {
        return new PartialFeatureGate(this.Key, this.InstrumentType, predicate);
    }

    public PartialFeatureGateAsync ControlledBy(Func<Task<bool>> predicate)
    {
        return new PartialFeatureGateAsync(this.Key, this.InstrumentType, predicate);
    }

    public class PartialFeatureGate
    {
        public PartialFeatureGate(string key, InstrumentType instrumentType, Func<bool> controlledBy)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

        public Func<bool> ControlledBy { get; }

        public FeatureGate WhenOpened(Action? action)
        {
            return new FeatureGate(this.Key, this.InstrumentType, this.ControlledBy, action, null);
        }

        public PartialFeatureGate<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return new PartialFeatureGate<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function);
        }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function, null);
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialFeatureGateAsync<TResult>(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function);
        }
    }

    public class PartialFeatureGateAsync
    {
        public PartialFeatureGateAsync(string key, InstrumentType instrumentType, Func<Task<bool>> controlledBy)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

        public Func<Task<bool>> ControlledBy { get; }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.InstrumentType, this.ControlledBy, function, null);
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialFeatureGateAsync<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function);
        }

        public FeatureGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        public PartialFeatureGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }

    public class PartialFeatureGate<TResult>
    {
        public PartialFeatureGate(string key, InstrumentType instrumentType, Func<bool> controlledBy, Func<TResult> whenOpened)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

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

    public class PartialFeatureGateAsync<TResult>
    {
        public PartialFeatureGateAsync(string key, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

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
