using System;
using System.Collections.Generic;
using System.Text;

namespace StreamInsights.Abstractions
{
    /// <summary>
    /// Get a single value from one or more values.
    /// </summary>
    public interface IValues
    {
        int Count { get; }

        object this[int index] { get; }
    }
}
