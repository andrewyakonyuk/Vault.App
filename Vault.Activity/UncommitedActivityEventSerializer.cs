using Orleans.CodeGeneration;
using Orleans.Serialization;
using System;
using Vault.Shared;

namespace Vault.Activity
{
    public partial class UncommitedActivityEvent
    {
        [CopierMethod]
        public static object DeepCopier(object original)
        {
            return original;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, BinaryTokenStreamWriter stream, Type expected)
        {
            var input = (UncommitedActivityEvent)untypedInput;
            SerializationManager.SerializeInner(input.Actor, stream, typeof(String));
            SerializationManager.SerializeInner(input.Content, stream, typeof(String));
            SerializationManager.SerializeInner(input.Id, stream, typeof(String));
            SerializationManager.SerializeInner(input.MetaBag, stream, typeof(DynamicDictionary));
            SerializationManager.SerializeInner(input.Provider, stream, typeof(String));
            SerializationManager.SerializeInner(input.Published, stream, typeof(DateTimeOffset));
            SerializationManager.SerializeInner(input.Target, stream, typeof(String));
            SerializationManager.SerializeInner(input.Title, stream, typeof(String));
            SerializationManager.SerializeInner(input.Uri, stream, typeof(String));
            SerializationManager.SerializeInner(input.Verb, stream, typeof(String));
            SerializationManager.SerializeInner(input.StreamId, stream, typeof(Guid));
            SerializationManager.SerializeInner(input.Bucket, stream, typeof(string));
        }

        [DeserializerMethod]
        public static Object Deserializer(Type expected, BinaryTokenStreamReader stream)
        {
            var result = new UncommitedActivityEvent();
            @DeserializationContext.@Current.@RecordObject(result);
            result.Actor = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Content = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Id = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.MetaBag = (Object)SerializationManager.@DeserializeInner(typeof(DynamicDictionary), stream);
            result.Provider = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Published = (DateTimeOffset)SerializationManager.@DeserializeInner(typeof(DateTimeOffset), stream);
            result.Target = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Title = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Uri = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.Verb = (String)SerializationManager.@DeserializeInner(typeof(String), stream);
            result.StreamId = (Guid)SerializationManager.DeserializeInner(typeof(Guid), stream);
            result.Bucket = (string)SerializationManager.DeserializeInner(typeof(string), stream);
            return result;
        }
    }
}
