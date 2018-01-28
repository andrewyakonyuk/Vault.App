using Newtonsoft.Json;

namespace StreamInsights.Abstractions
{
    /// <summary>
    /// A Link is an indirect, qualified reference to a resource identified by a URL. 
    /// The fundamental model for links is established by [ RFC5988]. 
    /// Many of the properties defined by the Activity Vocabulary allow values that 
    /// are either instances of Object or Link. When a Link is used, it establishes 
    /// a qualified relation connecting the subject (the containing object) 
    /// to the resource identified by the href. Properties of the Link 
    /// are properties of the reference as opposed to properties of the resource.
    /// </summary>
    public class ASLink
    {
        /// <summary>
        /// Provides the globally unique identifier 
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public Values<string> Id { get; set; }

        /// <summary>
        /// Identifies the Object type. Multiple values may be specified.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<string> Type { get; set; }

        /// <summary>
        /// The target resource pointed to by a Link.
        /// </summary>
        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        /// <summary>
        /// A link relation associated with a Link. The value must conform to 
        /// both the [HTML5] and [RFC5988] "link relation" definitions.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "rel", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<string> Rel { get; set; }

        /// <summary>
        /// Identifies the MIME media type of the referenced resource.
        /// </summary>
        [JsonProperty(PropertyName = "mediaType", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaType { get; set; }

        /// <summary>
        /// A simple, human-readable, plain-text name for the object. 
        /// HTML markup must not be included. The name may be expressed 
        /// using multiple language-tagged values.
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Hints as to the language used by the target resource. 
        /// </summary>
        [JsonProperty(PropertyName = "hrefLang", NullValueHandling = NullValueHandling.Ignore)]
        public string HrefLang { get; set; }

        /// <summary>
        /// Specifies a hint as to the rendering height in device-independent pixels 
        /// of the linked resource.
        /// </summary>
        [JsonProperty(PropertyName = "height", NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        /// <summary>
        ///  Specifies a hint as to the rendering width in device-independent pixels 
        ///  of the linked resource.
        /// </summary>
        [JsonProperty(PropertyName = "width", NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }

        /// <summary>
        /// Identifies an entity that provides a preview of this object.
        /// </summary>
        [JsonConverter(typeof(ValuesConverter))]
        [JsonProperty(PropertyName = "preview", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Values<string> Preview { get; set; }
    }
}
