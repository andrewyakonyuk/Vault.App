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
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var input = (UncommitedActivityEvent)untypedInput;
            SerializationManager.SerializeInner(input.Actor, context, typeof(String));
            SerializationManager.SerializeInner(input.Content, context, typeof(String));
            SerializationManager.SerializeInner(input.Id, context, typeof(String));
            SerializationManager.SerializeInner(input.MetaBag, context, typeof(DynamicDictionary));
            SerializationManager.SerializeInner(input.Provider, context, typeof(String));
            SerializationManager.SerializeInner(input.Published, context, typeof(DateTimeOffset));
            SerializationManager.SerializeInner(input.Target, context, typeof(String));
            SerializationManager.SerializeInner(input.Title, context, typeof(String));
            SerializationManager.SerializeInner(input.Uri, context, typeof(String));
            SerializationManager.SerializeInner(input.Verb, context, typeof(String));
            SerializationManager.SerializeInner(input.StreamId, context, typeof(Guid));
            SerializationManager.SerializeInner(input.Bucket, context, typeof(string));
        }

        [DeserializerMethod]
        public static Object Deserializer(Type expected, IDeserializationContext context)
        {
            var result = new UncommitedActivityEvent();
            context.@RecordObject(result);
            result.Actor = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Content = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Id = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.MetaBag = (Object)SerializationManager.@DeserializeInner(typeof(DynamicDictionary), context);
            result.Provider = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Published = (DateTimeOffset)SerializationManager.@DeserializeInner(typeof(DateTimeOffset), context);
            result.Target = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Title = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Uri = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.Verb = (String)SerializationManager.@DeserializeInner(typeof(String), context);
            result.StreamId = (Guid)SerializationManager.DeserializeInner(typeof(Guid), context);
            result.Bucket = (string)SerializationManager.DeserializeInner(typeof(string), context);
            return result;
        }
    }
}
