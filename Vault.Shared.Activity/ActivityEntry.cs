using System;
using Newtonsoft.Json;
using Vault.Shared;

namespace Vault.Shared.Activity
{
    [Serializable]
    public abstract class ActivityEntry
    {
        public ActivityEntry()
        {
            MetaBag = new DynamicDictionary();
        }

        /// <summary>
        /// Additional data for activity
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public dynamic MetaBag { get; set; }

        /// <summary>
        /// Provides a permanent, universally unique identifier for the activity
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identifies the action that the activity describes.
        /// </summary>
        public string Verb { get; set; }

        /// <summary>
        /// Describes the entity that performed the activity
        /// </summary>
        public string Actor { get; set; }

        /// <summary>
        /// Describes the target of the activity.
        /// The precise meaning of the activity's target is dependent on the activities verb,
        /// but will often be the object the English preposition "to".
        /// For instance, in the activity, "John saved a movie to his wishlist",
        /// the target of the activity is "wishlist"
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The date and time at which the activity was published
        /// </summary>
        public DateTimeOffset Published { get; set; }

        /// <summary>
        /// Describes the application that published the activity
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Natural-language description of the activity
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Natural-language title or headline for the activity
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// An IRI [RFC3987] identifying a resource providing an HTML representation of the activity
        /// </summary>
        public string Uri { get; set; }
    }
}