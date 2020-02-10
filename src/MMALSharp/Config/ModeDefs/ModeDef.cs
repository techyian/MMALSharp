using MMALSharp.Config.SensorRegs;
using System.Collections.Generic;

namespace MMALSharp.Config
{
    public class ModeDef
    {
        public List<SensorReg> Regs { get; set; }
        public int NumRegs { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Encoding { get; set; }
        public BayerOrder Order { get; set; }
        public int NativeBitDepth { get; set; }
        public byte ImageId { get; set; }
        public byte DataLanes { get; set; }
        public int MinVts { get; set; }
        public int LineTimeNs { get; set; }
        public int[] Timing { get; set; }
        public int[] Term { get; set; }
        public int BlackLevel { get; set; }
    }
}
