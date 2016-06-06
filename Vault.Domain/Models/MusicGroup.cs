using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Domain.Models
{
    /// <summary>
    /// A musical group, such as a band, an orchestra, or a choir. Can also be a solo musician.
    /// </summary>
    public class MusicGroup : Organization
    {
        List<MusicAlbum> _albums = new List<MusicAlbum>();

        public virtual ICollection<MusicAlbum> Albums { get { return _albums; } }

        /// <summary>
        /// Genre of the creative work or group.
        /// </summary>
        public virtual string Genre { get; set; }
    }
}