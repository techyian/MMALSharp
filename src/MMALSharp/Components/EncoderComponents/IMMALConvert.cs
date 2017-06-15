using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Handlers;

namespace MMALSharp.Components.EncoderComponents
{
    public interface IMMALConvert
    {
        Task Convert(int outputPort);
    }
}
