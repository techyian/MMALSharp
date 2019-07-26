using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    public interface IVideoCaptureHandler
    {
        MotionType MotionType { get; set; }
    }
}
