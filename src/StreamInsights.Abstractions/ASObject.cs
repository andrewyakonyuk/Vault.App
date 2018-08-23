using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamInsights.Abstractions
{
    public class ASObject
    {
        public ASObject()
        {
            ExtensionData = new JObject();
        }

        public ASObject(ASObject other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.ExtensionData == null)
                ExtensionData = new JObject();
            else
                ExtensionData = new JObject(other.ExtensionData);
            Id = other.Id;
            Attachment = other.Attachment?.ToValues();
            AttributedTo = other.AttributedTo?.ToValues();
            Audience = other.Audience?.ToValues();
            Bcc = other.Bcc?.ToValues();
            Bto = other.Bto?.ToValues();
            Cc = other.Cc?.ToValues();
            Content = other.Content;
            ContentMap = other.ContentMap?.ToDictionary(p => p.Key, p => p.Value);
            EndTime = other.EndTime;
            Generator = other.Generator;
            Icon = other.Icon;
            Image = other.Image?.ToValues();
            InReplyTo = other.InReplyTo?.ToValues();
            Location = other.Location?.ToValues();
            MediaType = other.MediaType;
            Name = other.Name;
            NameMap = other.NameMap?.ToDictionary(p => p.Key, p => p.Value);
            Preview = other.Preview?.ToValues();
            Published = other.Published;
            StartTime = other.StartTime;
            Summary = other.Summary;
            SummaryMap = other.SummaryMap?.ToDictionary(p => p.Key, p => p.Value);
            Tag = other.Tag?.ToValues();
            To = other.To?.ToValues();
            Type = other.Type?.ToValues();
            Updated = other.Updated;
            Url = other.Url?.ToValues();
        }

        /// <summary>
        /// Provides a permanent, universally unique identifier for the activity
        /// </summary>
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Identifies the action that the activity describes.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Type { get; set; }

        /// <summary>
        /// Identifies a resource attached or related to an object that potentially 
        /// requires special handling. The intent is to provide a model that is at least 
        /// semantically similar to attachments in email.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "attachment", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Attachment { get; set; }

        /// <summary>
        /// Identifies one or more entities to which this object is attributed. 
        /// The attributed entities might not be Actors. For instance, an object might be
        /// attributed to the completion of another activity.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "attributedTo", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> AttributedTo { get; set; }

        /// <summary>
        /// Identifies one or more entities that represent the total population of entities 
        /// for which the object can considered to be relevant.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "audience", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Audience { get; set; }

        /// <summary>
        /// Identifies the context within which the object exists or an activity was performed.
        /// The notion of "context" used is intentionally vague.The intended function is to serve 
        /// as a means of grouping objects and activities that share a common originating context 
        /// or purpose.An example could be all activities relating to a common project or event. 
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "context", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Context { get; set; }

        /// <summary>
        /// The content or textual representation of the Object encoded as a JSON string. 
        /// By default, the value of content is HTML. The mediaType property can be used 
        /// in the object to indicate a different content type.
        /// </summary>
        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "contentMap", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> ContentMap { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "nameMap", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> NameMap { get; set; }

        /// <summary>
        /// The date and time describing the actual or expected ending time of the object. 
        /// When used with an Activity object, for instance, the endTime property specifies 
        /// the moment the activity concluded or is expected to conclude.
        /// </summary>
        [JsonProperty(PropertyName = "endTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Identifies the entity (e.g. an application) that generated the object.
        /// </summary>
        [JsonProperty(PropertyName = "generator", NullValueHandling = NullValueHandling.Ignore)]
        public string Generator { get; set; }

        /// <summary>
        /// Indicates an entity that describes an icon for this object. 
        /// The image should have an aspect ratio of one (horizontal) to one (vertical) 
        /// and should be suitable for presentation at a small size.
        /// </summary>
        [JsonProperty(PropertyName = "icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// Indicates an entity that describes an image for this object. 
        /// Unlike the icon property, there are no aspect ratio or display 
        /// size limitations assumed.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Image { get; set; }

        /// <summary>
        /// Indicates one or more entities for which this object is considered a response.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "inReplyTo", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> InReplyTo { get; set; }

        /// <summary>
        /// Indicates one or more physical or logical locations associated with the object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "location", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Location { get; set; }

        /// <summary>
        /// Identifies an entity that provides a preview of this object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "preview", NullValueHandling = NullValueHandling.Ignore)]
        public Values<ASObject> Preview { get; set; }

        /// <summary>
        /// The date and time at which the object was published
        /// </summary>
        [JsonProperty(PropertyName = "published", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Published { get; set; }

        /// <summary>
        /// The date and time describing the actual or expected starting time of the object. 
        /// When used with an Activity object, for instance, the startTime property specifies 
        /// the moment the activity began or is scheduled to begin
        /// </summary>
        [JsonProperty(PropertyName = "startTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// A natural language summarization of the object encoded as HTML. 
        /// Multiple language tagged summaries may be provided.
        /// </summary>
        [JsonProperty(PropertyName = "summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "summaryMap", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> SummaryMap { get; set; }

        /// <summary>
        /// One or more "tags" that have been associated with an objects. 
        /// A tag can be any kind of Object. The key difference between attachment and tag 
        /// is that the former implies association by inclusion, while the latter implies 
        /// associated by reference.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "tag", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Tag { get; set; }

        /// <summary>
        /// The date and time at which the object was updated
        /// </summary>
        [JsonProperty(PropertyName = "updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Updated { get; set; }

        /// <summary>
        /// Identifies one or more links to representations of the object
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Url { get; set; }

        /// <summary>
        /// Identifies an entity considered to be part of the public primary audience of an Object
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "to", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> To { get; set; }

        /// <summary>
        /// Identifies an Object that is part of the private primary audience of this Object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "bto", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Bto { get; set; }

        /// <summary>
        /// Identifies an Object that is part of the public secondary audience of this Object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "cc", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Cc { get; set; }

        /// <summary>
        /// Identifies one or more Objects that are part of 
        /// the private secondary audience of this Object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "bcc", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Bcc { get; set; }

        /// <summary>
        ///  identifies the MIME media type of the value of the content property. If not specified, 
        ///  the content property is assumed to contain text/html content.
        /// </summary>
        [JsonProperty(PropertyName = "mediaType", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaType { get; set; }

        /// <summary>
        /// Additional data for activity
        /// </summary>
        [JsonExtensionData]
        public JObject ExtensionData { get; set; }
    }
}
