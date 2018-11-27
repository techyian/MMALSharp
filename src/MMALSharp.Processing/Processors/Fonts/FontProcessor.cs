using System;
using System.Drawing;
using MMALSharp.Common;

namespace MMALSharp.Processors
{
    // TODO Parse OpenType fonts.
    public class FontProcessor : IFrameProcessor
    {
        public string Text { get; }

        public FontProcessor(string text, Color fontColour)
        {
            this.Text = text;
        }

        public void Apply(byte[] store, IImageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
