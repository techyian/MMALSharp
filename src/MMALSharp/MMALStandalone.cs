using System;
using System.Collections.Generic;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Used for Standalone use of MMALSharp without camera.
    /// </summary>
    public class MMALStandalone
    {
        /// <summary>
        /// Bootstraps MMAL for standalone use.
        /// </summary>
        public static MMALStandalone Instance => Lazy.Value;

        private static readonly Lazy<MMALStandalone> Lazy = new Lazy<MMALStandalone>(() => new MMALStandalone());
        
        private MMALStandalone()
        {
            BcmHost.bcm_host_init();

            MMALLog.ConfigureLogger();
        }
        
        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done with MMAL.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.Debug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(MMALBootstrapper.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());
            
            BcmHost.bcm_host_deinit();
        }
    }
}