using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Activity.Persistence
{
    public static class SerializationExtensions
    {
        public static byte[] Serialize<T>(this JsonSerializer serializer, T value)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(this JsonSerializer serializer, byte[] serialized)
        {
            serialized = serialized ?? new byte[] { };
            if (serialized.Length == 0)
            {
                return default(T);
            }

            using (var stream = new MemoryStream(serialized))
                return serializer.Deserialize<T>(stream);
        }
    }
}
