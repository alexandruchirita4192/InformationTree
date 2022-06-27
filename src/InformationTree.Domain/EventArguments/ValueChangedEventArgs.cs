using System;

namespace InformationTree.Domain.EventArguments
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T LastValue { get; private set; }
        public T NewValue { get; private set; }

        public ValueChangedEventArgs(T lastValue, T newValue)
        {
            LastValue = lastValue;
            NewValue = newValue;
        }
    }
}