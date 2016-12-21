using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    class Program
    {
        public static void Main(string[] args)
        {
            BcmHost.bcm_host_init();

            MMALComponentBase.CreateComponent(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA);
            
            BcmHost.bcm_host_deinit();
        }
    }
}
