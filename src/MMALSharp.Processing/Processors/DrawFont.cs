using System;
using System.Drawing;

namespace MMALSharp.Processors
{
    // TODO Parse OpenType fonts.
    public class DrawFont : IManipulate
    {
        public string Text { get; }

        public DrawFont(string text, Color fontColour)
        {
            this.Text = text;
        }

        public void Apply(byte[] store)
        {
            throw new NotImplementedException();
        }
    }
}
