using System;
namespace TAgent
{
	public class Utils
	{
		public Utils()
		{
		}
	}
    public enum OptionStatus
    {
        None,
        Some
    }

    public struct Option<T>
    {
        private readonly T value;

        public Option(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public bool HasValue { get; }
        public T Value => HasValue ? value : throw new InvalidOperationException("No value present.");

        public static Option<T> None => new Option<T>();
    }
}

