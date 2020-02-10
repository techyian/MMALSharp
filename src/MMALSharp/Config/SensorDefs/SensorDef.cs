using MMALSharp.Config.SensorRegs;
using System.Collections.Generic;

namespace MMALSharp.Config.SensorDefs
{
    public class SensorDef
    {
        public string Name { get; set; }
        public List<ModeDef> Modes { get; set; }
        public int NumModes { get; set; }
        public List<SensorReg> StopReg { get; set; }
        public int NumStopRegs { get; set; }

        public byte I2CAddr { get; set; }
        public int I2CAddressing { get; set; }
        public int I2CDataSize { get; set; }

        public ushort I2CIdentLength { get; set; }
        public ushort I2CIdentReg { get; set; }
        public ushort I2CIdentValue { get; set; }

        public ushort VFlipReg { get; set; }
        public int VFlipRegBit { get; set; }
        public ushort HFlipReg { get; set; }
        public int HFlipRegBit { get; set; }
        public int FlipsDontChangeBayerOrder { get; set; }

        public ushort ExposureReg { get; set; }
        public int ExposureRegNumBits { get; set; }

        public ushort VtsReg { get; set; }
        public int VtsRegNumBits { get; set; }

        public ushort GainReg { get; set; }
        public int GainRegNumBits { get; set; }
    }
}
