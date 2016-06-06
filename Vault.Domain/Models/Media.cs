using System;

namespace Vault.Domain.Models
{
    public abstract class Media : CreativeWork
    {
        /// <summary>
        /// Date when this media object was uploaded.
        /// </summary>
        public virtual DateTime UploadDate { get; set; }

        /// <summary>
        /// The width of the item.
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// The height of the item.
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// mp3, mpeg4, etc.
        /// </summary>
        public virtual string EncodingFormat { get; set; }

        /// <summary>
        /// File size in (mega/kilo) bytes.
        /// </summary>
        public virtual string ContentSize { get; set; }
    }
}