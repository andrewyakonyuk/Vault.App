using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.CodeGeneration;
using Orleans.Serialization;
using Vault.Shared;

namespace Vault.Activity.Host
{
    [Serializer(typeof(ActivityEvent)), RegisterSerializer]
    internal class ActivityEventSerializer
    {
        [CopierMethod]
        public static Object DeepCopier(Object original)
        {
            return original;
        }

        [SerializerMethod]
        public static void Serializer(Object untypedInput, BinaryTokenStreamWriter stream, Type expected)
        {
            ActivityEvent input = (ActivityEvent)untypedInput;
            SerializationManager.SerializeInner(input.@Actor, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Content, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Id, stream, typeof(String));
            SerializationManager.SerializeInner(input.@MetaBag, stream, typeof(DynamicDictionary));
            SerializationManager.SerializeInner(input.@Provider, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Published, stream, typeof(DateTimeOffset));
            SerializationManager.SerializeInner(input.@Target, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Title, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Uri, stream, typeof(String));
            SerializationManager.SerializeInner(input.@Verb, stream, typeof(String));
        }

        [DeserializerMethod]
        public static Object Deserializer(Type expected, BinaryTokenStreamReader stream)
        {
            ActivityEvent result = new ActivityEvent();
            DeserializationContext.@Current.@RecordObject(result);
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
            return (ActivityEvent)result;
        }

        public static void Register()
        {
            SerializationManager.@Register(typeof(ActivityEvent), DeepCopier, Serializer, Deserializer);
        }

        static ActivityEventSerializer()
        {
            Register();
        }
    }
}