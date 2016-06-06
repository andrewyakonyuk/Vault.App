using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    /// <summary>
    /// A collection of music tracks in playlist form.
    /// </summary>
    public abstract class MusicPlaylist : CreativeWork
    {
        readonly List<MusicRecording> _tracks;

        public MusicPlaylist()
        {
            _tracks = new List<MusicRecording>();
        }

        public virtual int NumTracks { get; set; }

        public virtual ICollection<MusicRecording> Tracks { get { return _tracks; } }
    }
}