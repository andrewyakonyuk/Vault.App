using Newtonsoft.Json;
using System;

namespace StreamInsights.Abstractions
{
    [Serializable]
    public class Activity : ASObject
    {
        public Activity()
        {
        }

        public Activity(Activity other)
            : base(other)
        {
            Actor = other.Actor;
            Instrument = other.Instrument;
            Object = other.Object;
            Origin = other.Origin;
            Result = other.Result;
            Target = other.Target;
        }

        /// <summary>
        /// Describes the entity that performed the activity
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "actor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Actor { get; set; }

        /// <summary>
        /// describes the direct object of the activity. For instance, 
        /// in the activity "John added a movie to his wishlist", the object of the activity 
        /// is the movie added.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "object", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Object { get; set; }

        /// <summary>
        /// Describes the indirect object, or target, of the activity. 
        /// The precise meaning of the target is largely dependent on 
        /// the type of action being described but will often be the object of 
        /// the English preposition "to". For instance, 
        /// in the activity "John added a movie to his wishlist", the target of the activity is 
        /// John's wishlist. An activity can have more than one target.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "target", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Target { get; set; }

        /// <summary>
        /// Describes an indirect object of the activity from which the activity is directed. 
        /// The precise meaning of the origin is the object of the English preposition "from".
        /// For instance, in the activity "John moved an item to List B from List A", 
        /// the origin of the activity is "List A".
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "origin", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Origin { get; set; }

        /// <summary>
        /// Describes the result of the activity. For instance, 
        /// if a particular action results in the creation of a new resource, 
        /// the result property can be used to describe that new resource.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "result", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Result { get; set; }

        /// <summary>
        /// Identifies one or more objects used (or to be used) in the completion of an Activity.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "instrument", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<ASObject> Instrument { get; set; }
    }
}
