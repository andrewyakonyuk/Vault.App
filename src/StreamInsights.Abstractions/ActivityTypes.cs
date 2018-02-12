using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamInsights.Abstractions
{
    public class ActivityTypes
    {
        /// <summary>
        /// Indicates that the actor accepts the object. The target property can be used in certain 
        /// circumstances to indicate the context into which the object has been accepted.
        /// </summary>
        public const string Accept = "accept";

        /// <summary>
        /// Indicates that the actor has added the object to the target. If the target property 
        /// is not explicitly specified, the target would need to be determined implicitly by context. 
        /// The origin can be used to identify the context from which the object originated.
        /// </summary>
        public const string Add = "add";

        /// <summary>
        /// Indicates that the actor is calling the target's attention the object.
        /// </summary>
        public const string Announce = "announce";

        /// <summary>
        /// An IntransitiveActivity that indicates that the actor has arrived at the location. 
        /// The origin can be used to identify the context from which the actor originated. 
        /// The target typically has no defined meaning.
        /// </summary>
        public const string Arrive = "arrive";

        /// <summary>
        /// Indicates that the actor is blocking the object. Blocking is a stronger form of Ignore. 
        /// The typical use is to support social systems that allow one user to block activities or 
        /// content of other users. The target and origin typically have no defined meaning.
        /// </summary>
        public const string Block = "block";

        /// <summary>
        /// Indicates that the actor has created the object.
        /// </summary>
        public const string Create = "create";

        /// <summary>
        /// Indicates that the actor has deleted the object. If specified, the origin indicates 
        /// the context from which the object was deleted.
        /// </summary>
        public const string Delete = "delete";

        /// <summary>
        /// Indicates that the actor dislikes the object.
        /// </summary>
        public const string Dislike = "dislike";

        /// <summary>
        /// Indicates that the actor is "flagging" the object. Flagging is defined 
        /// in the sense common to many social platforms as reporting content as being inappropriate 
        /// for any number of reasons.
        /// </summary>
        public const string Flag = "flag";

        /// <summary>
        /// Indicates that the actor is "following" the object. Following is defined in the sense 
        /// typically used within Social systems in which the actor is interested 
        /// in any activity performed by or on the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Follow = "follow";

        /// <summary>
        /// Indicates that the actor is ignoring the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Ignore = "ignore";

        /// <summary>
        /// A specialization of Offer in which the actor is extending an invitation 
        /// for the object to the target.
        /// </summary>
        public const string Invite = "invite";

        /// <summary>
        /// Indicates that the actor has joined the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Join = "join";

        /// <summary>
        /// Indicates that the actor has left the object. 
        /// The target and origin typically have no meaning
        /// </summary>
        public const string Leave = "leave";

        /// <summary>
        /// Indicates that the actor likes, recommends or endorses the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Like = "like";

        /// <summary>
        /// Indicates that the actor has listened to the object.
        /// </summary>
        public const string Listen = "listen";

        /// <summary>
        /// Indicates that the actor has moved object from origin to target. 
        /// If the origin or target are not specified, either can be determined by context.
        /// </summary>
        public const string Move = "move";

        /// <summary>
        /// Indicates that the actor is offering the object. 
        /// If specified, the target indicates the entity to which the object is being offered.
        /// </summary>
        public const string Offer = "Offer";

        /// <summary>
        /// Indicates that the actor is rejecting the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Reject = "reject";

        /// <summary>
        /// Indicates that the actor has read the object.
        /// </summary>
        public const string Read = "read";

        /// <summary>
        /// Indicates that the actor is removing the object. 
        /// If specified, the origin indicates the context from which the object is being removed.
        /// </summary>
        public const string Remove = "remove";

        /// <summary>
        /// A specialization of Reject in which the rejection is considered tentative.
        /// </summary>
        public const string TentativeReject = "tentativereject";

        /// <summary>
        /// A specialization of Accept indicating that the acceptance is tentative.
        /// </summary>
        public const string TentativeAccept = "tentativeaccept";

        /// <summary>
        /// Indicates that the actor is traveling to target from origin. Travel is an IntransitiveObject 
        /// whose actor specifies the direct object. 
        /// If the target or origin are not specified, either can be determined by context.
        /// </summary>
        public const string Travel = "travel";

        /// <summary>
        /// Indicates that the actor is undoing the object. In most cases, the object 
        /// will be an Activity describing some previously performed action 
        /// (for instance, a person may have previously "liked" an article but, for whatever reason, 
        /// might choose to undo that like at some later point in time).
        /// </summary>
        public const string Undo = "undo";

        /// <summary>
        /// Indicates that the actor stop like, recommend or endorse the object. 
        /// The target and origin typically have no defined meaning.
        /// </summary>
        public const string Unlike = "unlike";

        /// <summary>
        /// Indicates that the actor has updated the object. Note, however, 
        /// that this vocabulary does not define a mechanism for describing the actual set
        /// of modifications made to object.
        /// </summary>
        public const string Update = "update";

        /// <summary>
        /// Indicates that the actor has viewed the object.
        /// </summary>
        public const string View = "view";
    }
}
