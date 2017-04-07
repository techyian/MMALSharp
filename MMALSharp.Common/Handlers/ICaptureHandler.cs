using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface ICaptureHandler : IDisposable
    {
        /// <summary>
        /// Used to process the byte array containing our image data
        /// </summary>
        /// <param name="data">A byte array containing image data</param>
        void Process(byte[] data);     
        /// <summary>
        /// Used for any further processing once we have completed capture
        /// </summary>
        void PostProcess();
    }
}
