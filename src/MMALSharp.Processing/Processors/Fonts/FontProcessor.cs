using System;
using System.Drawing;
using MMALSharp.Common;

namespace MMALSharp.Processors
{
    /// <summary>
    /// WIP. TODO Parse OpenType fonts.
    /// </summary>
    public class FontProcessor : IFrameProcessor
    {
        /// <summary>
        /// The text to write.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="FontProcessor"/>.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="fontColour">The color of the font.</param>
        public FontProcessor(string text, Color fontColour)
        {
            this.Text = text;
        }

        /// <inheritdoc />
        public void Apply(IImageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
