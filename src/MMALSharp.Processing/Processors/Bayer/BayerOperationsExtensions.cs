using MMALSharp.Common;
using MMALSharp.Processors.Bayer;

namespace MMALSharp.Processors
{
    public static class BayerOperationsExtensions
    {
        public static IFrameProcessingContext StripBayerMetadata(this IFrameProcessingContext context, CameraVersion version)
            => context.Apply(new BayerMetaProcessor(version));
        
        public static IFrameProcessingContext Demosaic(this IFrameProcessingContext context)
            => context.Apply(new DemosaicProcessor());
    }
}
