using System;
using System.Drawing;

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

        public void Apply(byte[] store)
        {
            throw new NotImplementedException();
        }
    }
}
