using Orleans.CodeGeneration;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;

namespace Vault.Activity
{
    public partial class ActivityEventAttempt
    {
        [CopierMethod]
        public static object DeepCopier(object original)
        {
            return original;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var input = (CommitedActivityEvent)untypedInput;
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
        }

        [DeserializerMethod]
        public static Object Deserializer(Type expected, IDeserializationContext context)
        {
            var result = new CommitedActivityEvent();
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
            return result;
        }
    }
}
