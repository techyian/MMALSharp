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

        public int I2CAddr { get; set; }
        public int I2CAddressing { get; set; }
        public int I2CDataSize { get; set; }

        public int I2CIdentLength { get; set; }
        public int I2CIdentReg { get; set; }
        public int I2CIdentValue { get; set; }

        public int VFlipReg { get; set; }
        public int VFlipRegBit { get; set; }
        public int HFlipReg { get; set; }
        public int HFlipRegBit { get; set; }
        public int FlipsDontChangeBayerOrder { get; set; }

        public int ExposureReg { get; set; }
        public int ExposureRegNumBits { get; set; }

        public int VtsReg { get; set; }
        public int VtsRegNumBits { get; set; }

        public int GainReg { get; set; }
        public int GainRegNumBits { get; set; }
    }
}
