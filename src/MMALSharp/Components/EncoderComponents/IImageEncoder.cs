using MMALSharp.Config;

namespace MMALSharp.Components.EncoderComponents
{
    /// <summary>
    /// Represents an image encoder component.
    /// </summary>
    public interface IImageEncoder : IEncoder
    {
        /// <summary>
        /// Flag to return raw Bayer metadata with JPEG frames.
        /// </summary>
        bool RawBayer { get; }

        /// <summary>
        /// Flag to add EXIF tags to image frames.
        /// </summary>
        bool UseExif { get; }

        /// <summary>
        /// An array of user provided EXIF tags.
        /// </summary>
        ExifTag[] ExifTags { get; }

        /// <summary>
        /// When enabled and if configured, image frames will be quickly processed by the camera's video port.
        /// </summary>
        bool ContinuousCapture { get; }

        /// <summary>
        /// The JPEG thumbnail configuration object.
        /// </summary>
        JpegThumbnail JpegThumbnailConfig { get; set; }
    }
}
