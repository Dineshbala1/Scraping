using System;

namespace BigbossScraping.Domain
{
    public class CategoryNotChangedException : Exception
    {
        public CategoryNotChangedException(string message) : base(message) { }
    }

    public class NotFound<T> : Exception where T : class
    {
        public T DefaultValue { get; }

        public NotFound(string message) : base(message)
        {
            DefaultValue = default(T);
        }
    }
}
