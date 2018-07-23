using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Linq;

namespace StreamInsights.Abstractions
{
    /// <summary>
    /// Converts an <see cref="IValue"/> object to JSON.
    /// </summary>
    /// <seealso cref="JsonConverter" />
    public class ValuesConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            var result = objectType == typeof(IValues);
            return result;
        }

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var isNullable = TypeUtils.IsNullableType(objectType);

            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined)
            {
                if (isNullable)
                    return null;
                else
                {
                    throw new InvalidCastException();
                }
            }

            var targetType = objectType;
            if (isNullable)
                targetType = Nullable.GetUnderlyingType(objectType);

            var genericArgs = targetType.GenericTypeArguments;
            if (genericArgs.Length > 2)
                throw new NotSupportedException();

            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                var indexOf = genericArgs.Length == 1 ? 0 : 1;
                var resultType = TypeUtils.MakeGenericType(typeof(List<>), genericArgs[indexOf]);

                var contract = serializer.ContractResolver.ResolveContract(resultType);
                if (contract.DefaultCreator != null)
                {
                    existingValue = contract.DefaultCreator();
                }

                // Using "populate" avoids infinite recursion.
                using (var subReader = token.CreateReader())
                {
                    serializer.Populate(subReader, existingValue);
                }

                return TypeUtils.Cast(resultType, targetType, existingValue);
            }
            else if (token.Type == JTokenType.Object)
            {
                var resultType = genericArgs.FirstOrDefault(t => !TypeUtils.IsSimpleType(t));
                if (resultType == null)
                {
                    throw new InvalidOperationException($"Expect any of {string.Join<Type>(", ", genericArgs)} but see 'Object' at the path: '{token.Path}' ");
                }

                var contract = serializer.ContractResolver.ResolveContract(resultType);
                if (contract.DefaultCreator != null)
                {
                    existingValue = contract.DefaultCreator();
                }

                // Using "populate" avoids infinite recursion.
                using (var subReader = token.CreateReader())
                {
                    serializer.Populate(subReader, existingValue);
                }
                    
                return TypeUtils.Cast(resultType, targetType, existingValue);
            }
            else
            {
                var complexTypes = genericArgs.Where(t => !TypeUtils.IsSimpleType(t)).ToArray();

                if (complexTypes.Length < genericArgs.Length)
                {
                    // simple way. there is at least one a simple type
                    using (var subReader = token.CreateReader())
                    {
                        var value = serializer.Deserialize(subReader, targetType);
                        return value;
                    }
                }
                else
                {
                    //todo: bad - special handling
                    if (token.Type == JTokenType.String && complexTypes.Any(t => typeof(ASObject).IsAssignableFrom(t)))
                    {
                        var idValue = token.ToObject<string>(); 
                        var value = new ASObject
                        {
                            Id = idValue
                        };
                        return (Values<ASObject>)value;
                    }
                    else throw new InvalidCastException();
                }
            }
        }

        /// <summary>
        /// Writes the specified <see cref="IValue"/> object using the JSON writer.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The <see cref="IValue"/> object.</param>
        /// <param name="serializer">The JSON serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var values = (IValues)value;
            if (values == null || values.Count == 0)
            {
                writer.WriteNull();
            }
            else if (values.Count == 1)
            {
                WriteObject(writer, values[0], serializer);
            }
            else
            {
                WriteObject(writer, values, serializer);
            }
        }

        /// <summary>
        /// Writes the object retrieved from <see cref="IValue"/> when one is found.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The JSON serializer.</param>
        public virtual void WriteObject(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value, serializer);
            token.WriteTo(writer);
        }
    }

    public class TypeUtils
    {
        static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> ImplicitCache = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
        static readonly ConcurrentDictionary<Tuple<Type, Type>, Type> GenericCache = new ConcurrentDictionary<Tuple<Type, Type>, Type>();

        public static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(Enum),
            typeof(String),
            typeof(Decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }

        public static bool IsNullableType(Type t)
        {
            return (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static object Cast(Type initial, Type target, object value)
        {
            MethodInfo castMethodInfo = ImplicitCache.GetOrAdd(Tuple.Create(initial, target),
                key => key.Item2.GetMethod("op_Implicit", new[] { key.Item1 }));
            return castMethodInfo.Invoke(null, new[] { value });
        }

        public static Type MakeGenericType(Type generic, Type arg0)
        {
            return GenericCache.GetOrAdd(Tuple.Create(generic, arg0),
                k => k.Item1.MakeGenericType(k.Item2));
        }
    }
}
