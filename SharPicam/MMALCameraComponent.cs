using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public class MMALCameraComponent : MMALComponentBase
    {
        public MMALCameraComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_CAMERA) { }

        public MMALCameraComponent(string name) : base(name)
        {
        }
    }
}
