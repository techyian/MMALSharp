using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public interface ICameraInfoComponent : IComponent
    {
        string SensorName { get; }
        int MaxWidth { get; }
        int MaxHeight { get; }
    }
}
