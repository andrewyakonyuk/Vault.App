using System;
using System.Collections.Generic;
using System.Text;

namespace StreamInsights.Abstractions
{
    [Serializable]
    public class UncommitedActivity : Activity
    {
        public UncommitedActivity()
        {

        }

        public UncommitedActivity(Activity other)
            : base(other)
        {

        }

        public UncommitedActivity(UncommitedActivity other) :
            base(other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            StreamId = other.StreamId;
            Bucket = other.Bucket;
            OwnerId = other.OwnerId;
        }

        /// <summary>
        ///     Gets the value which uniquely identifies the stream to which the activity belongs.
        /// </summary>
        public string StreamId { get; set; }

        /// <summary>
        ///     Gets the value which identifies bucket to which the activity belongs.
        /// </summary>
        public string Bucket { get; set; }

        /// <summary>
        /// Gets or sets the value which identifies owner to whom the activity belogns.
        /// </summary>
        public string OwnerId { get; set; }
    }
}
