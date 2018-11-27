using System.Drawing;

namespace MMALSharp.Processors.Fonts
{
    public static class FontOperationsExtensions
    {
        public static IFrameProcessingContext DrawFont(this IFrameProcessingContext context, string text, Color c)
            => context.Apply(new FontProcessor(text, c));
    }
}
