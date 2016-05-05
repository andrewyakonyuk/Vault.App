namespace Vault.Framework.Models
{
    /// <summary>
    /// An image file.
    /// </summary>
    public class Image : Media
    {
        /// <summary>
        /// The caption for this object.
        /// </summary>
        public virtual string Caption { get; set; }

        /// <summary>
        /// Thumbnail image for an image or video.
        /// </summary>
        public virtual Image Thumbnail { get; set; }
    }
}