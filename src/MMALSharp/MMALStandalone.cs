using System;
using System.Collections.Generic;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp
{
    public class MMALStandalone
    {
        /// <summary>
        /// Bootstraps MMAL for standalone use.
        /// </summary>
        public static MMALStandalone Instance => Lazy.Value;

        private static readonly Lazy<MMALStandalone> Lazy = new Lazy<MMALStandalone>(() => new MMALStandalone());
        
        /// <summary>
        /// List of all encoders currently in the pipeline.
        /// </summary>
        public List<MMALDownstreamComponent> DownstreamComponents { get; }
        
        private MMALStandalone()
        {
            BcmHost.bcm_host_init();

            MMALLog.ConfigureLogger();
            
            this.DownstreamComponents = new List<MMALDownstreamComponent>();
        }
        
        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done on the camera.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.Debug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(this.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());
            
            BcmHost.bcm_host_deinit();
        }
    }
}