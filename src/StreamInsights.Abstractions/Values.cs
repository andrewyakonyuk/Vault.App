using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public class Values
    {
        public static bool IsNullOrEmpty(IValues values)
        {
            return values == null || values.Count == 0;
        }

        public static Values<T> New<T>(T item)
        {
            if (item == null)
                return Values<T>.Empty;

            return new Values<T>(item);
        }

        public static Values<T> New<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
                return Values<T>.Empty;

            return new Values<T>(list);
        }

        public static Values<T> New<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                return Values<T>.Empty;

            return new Values<T>(array);
        }
    }
}
