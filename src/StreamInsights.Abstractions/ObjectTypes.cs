using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public class ObjectTypes
    {
        /// <summary>
        /// Represents any kind of multi-paragraph written work.
        /// </summary>
        public const string Article = "article";

        /// <summary>
        /// Represents an audio document of any kind.
        /// </summary>
        public const string Audio = "audio";

        /// <summary>
        /// Represents a document of any kind.
        /// </summary>
        public const string Document = "document";

        /// <summary>
        /// Represents any kind of event.
        /// </summary>
        public const string Event = "event";

        /// <summary>
        /// An image document of any kind
        /// </summary>
        public const string Image = "image";

        /// <summary>
        /// Represents a short written work typically less than a single paragraph in length.
        /// </summary>
        public const string Note = "note";

        /// <summary>
        /// Represents a Web Page.
        /// </summary>
        public const string Page = "page";

        /// <summary>
        /// Represents a logical or physical location.
        /// </summary>
        public const string Place = "place";

        /// <summary>
        /// A Profile is a content object that describes another Object, 
        /// typically used to describe Actor Type objects. The describes property is used to 
        /// reference the object being described by the profile.
        /// </summary>
        public const string Profile = "profile";

        /// <summary>
        /// Describes a relationship between two individuals. The subject and object properties 
        /// are used to identify the connected individuals.
        /// </summary>
        public const string Relationship = "relationship";

        /// <summary>
        /// A Tombstone represents a content object that has been deleted. 
        /// It can be used in Collections to signify that there used to be an object at this position, 
        /// but it has been deleted.
        /// </summary>
        public const string Thumstone = "thumbstone";

        /// <summary>
        /// Represents a video document of any kind.
        /// </summary>
        public const string Video = "video";
    }
}
