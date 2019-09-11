using System.Drawing;

namespace MMALSharp.Processors.Fonts
{
    /// <summary>
    /// WIP.
    /// </summary>
    public static class FontOperationsExtensions
    {
        /// <summary>
        /// WIP.
        /// </summary>
        /// <param name="context">The image context.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="c">The color of the font.</param>
        /// <returns>The current <see cref="IFrameProcessingContext"/>.</returns>
        public static IFrameProcessingContext DrawFont(this IFrameProcessingContext context, string text, Color c)
            => context.Apply(new FontProcessor(text, c));
    }
}
