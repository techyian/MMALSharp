using MMALSharp.Config.ModeDefs;
using MMALSharp.Config.SensorRegs;

namespace MMALSharp.Config.SensorDefs
{
    public static class Ov5647SensorDefs
    {
        public static SensorDef Ov5647SensorDef = new SensorDef
        {
            Name = "ov5647",
            Modes = Ov5647ModeDefs.ModeDefs,
            NumModes = Ov5647ModeDefs.ModeDefs.Count,
            StopReg = Ov5647SensorRegs.Ov5647StopRegs,
            NumStopRegs = Ov5647SensorRegs.Ov5647StopRegs.Count,

            I2CAddr = 0x36,
            I2CAddressing = 2,
            I2CIdentLength = 2,
            I2CIdentReg = 0x300A,
            I2CIdentValue = 0x4756,
            
            VFlipReg = 0x3820,
            VFlipRegBit = 0,
            HFlipReg = 0x3821,
            HFlipRegBit = 0,

            ExposureReg = 0x3500,
            ExposureRegNumBits = 20,

            VtsReg = 0x380E,
            VtsRegNumBits = 16,

            GainReg = 0x350A,
            GainRegNumBits = 10
        };
    }
}
