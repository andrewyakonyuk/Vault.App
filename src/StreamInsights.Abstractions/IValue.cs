using System;
using System.Collections.Generic;
using System.Text;

namespace StreamInsights.Abstractions
{
    /// <summary>
    /// Get a single value from one or more values.
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// Gets the non-null object representing the instance.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has a single or list of values.
        /// </summary>
        bool HasValue { get; }
    }
}
