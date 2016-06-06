using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    /// <summary>
    /// A collection of music tracks.
    /// </summary>
    public class MusicAlbum : CreativeWork
    {
        /// <summary>
        /// The artist that performed this album or recording.
        /// </summary>
        public virtual MusicGroup ByArtist { get; set; }

        /// <summary>
        /// Classification of the album by it's type of content: soundtrack, live album, studio album, etc.
        /// </summary>
        public virtual MusicAlbumProductionType ProductionType { get; set; }

        /// <summary>
        /// The kind of release which this album is: single, EP or album.
        /// </summary>
        public virtual MusicAlbumReleaseType ReleaseType { get; set; }
    }

    /// <summary>
    /// Classification of the album by it's type of content: soundtrack, live album, studio album, etc.
    /// </summary>
    public enum MusicAlbumProductionType
    {
        StudioAlbum = 0,
        CompilationAlbum,
        DJMixAlbum,
        DemoAlbum,
        LiveAlbum,
        MixtapeAlbum,
        RemixAlbum,
        SoundtrackAlbum,
        SpokenWordAlbum
    }

    /// <summary>
    /// The kind of release which this album is: single, EP or album.
    /// </summary>
    public enum MusicAlbumReleaseType
    {
        AlbumRelease,
        BroadcastRelease,
        EPRelease,
        SingleRelease
    }
}