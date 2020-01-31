using MMALSharp.Config.ModeDefs;
using MMALSharp.Config.SensorRegs;

namespace MMALSharp.Config.SensorDefs
{
    public static class Imx219SensorDefs
    {
        public static SensorDef Imx219SensorDef = new SensorDef
        {
            Name = "imx219",
            Modes = Imx219ModeDefs.ModeDefs,
            NumModes = Imx219ModeDefs.ModeDefs.Count,
            StopReg = Imx219SensorRegs.Imx219StopRegs,
            NumStopRegs = Imx219SensorRegs.Imx219StopRegs.Count,

            I2CAddr = 0x10,
            I2CAddressing = 2,
            I2CIdentLength = 2,
            I2CIdentReg = 0x0000,
            I2CIdentValue = 0x1902, // 0x0219 bytes reversed.

            VFlipReg = 0x172,
            VFlipRegBit = 1,
            HFlipReg = 0x172,
            HFlipRegBit = 0,

            ExposureReg = 0x015A,
            ExposureRegNumBits = 16,

            VtsReg = 0x0160,
            VtsRegNumBits = 16,

            GainReg = 0x0157,
            GainRegNumBits = 8 // Only valid up to 230.
        };
    }
}
