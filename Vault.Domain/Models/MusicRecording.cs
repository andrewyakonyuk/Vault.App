using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    /// <summary>
    /// A music recording (track), usually a single song.
    /// </summary>
    public class MusicRecording : CreativeWork
    {
        /// <summary>
        /// The duration of the item
        /// </summary>
        public virtual TimeSpan Duration { get; set; }

        /// <summary>
        /// The International Standard Recording Code for the recording.
        /// </summary>
        public virtual string IsrcCode { get; set; }

        /// <summary>
        /// The artist that performed this album or recording.
        /// </summary>
        public virtual MusicGroup ByArtist { get; set; }

        /// <summary>
        /// The album to which this recording belongs.
        /// </summary>
        public virtual MusicAlbum InAlbum { get; set; }
    }
}