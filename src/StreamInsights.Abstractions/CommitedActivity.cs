using System;

namespace StreamInsights.Abstractions
{
    [Serializable]
    public class CommitedActivity : Activity, ICommittable
    {
        public CommitedActivity()
        {

        }

        public CommitedActivity(Activity other)
            : base(other)
        {

        }

        public CommitedActivity(CommitedActivity other) :
            base(other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            StreamId = other.StreamId;
            Bucket = other.Bucket;
            CheckpointToken = other.CheckpointToken;
        }

        public long CheckpointToken { get; set; }

        /// <summary>
        ///     Gets the value which uniquely identifies the stream to which the activity belongs.
        /// </summary>
        public string StreamId { get; set; }

        /// <summary>
        ///     Gets the value which identifies bucket to which the the activity belongs.
        /// </summary>
        public string Bucket { get; set; }
    }
}
