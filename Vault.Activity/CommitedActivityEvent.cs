using System;

namespace Vault.Activity
{
    [Serializable]
    public partial class CommitedActivityEvent : ActivityEntry
    {
        public long CheckpointToken { get; set; }

        /// <summary>
        ///     Gets the value which uniquely identifies the stream to which the activity belongs.
        /// </summary>
        public Guid StreamId { get; set; }

        /// <summary>
        ///     Gets the value which identifies bucket to which the the activity belongs.
        /// </summary>
        public string Bucket { get; set; }
    }
}
