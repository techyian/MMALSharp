using MMALSharp.Config.SensorRegs;
using System.Collections.Generic;

namespace MMALSharp.Config.ModeDefs
{
    public static class Ov5647ModeDefs
    {
        public static readonly List<ModeDef> ModeDefs = new List<ModeDef>
        {
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Regs.Count,
                Width = 2592,
                Height = 1944,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1968,
                LineTimeNs = 32503,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode1Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode1Regs.Count,
                Width = 1920,
                Height = 1080,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1104,
                LineTimeNs = 29584,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode2Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode2Regs.Count,
                Width = 2592,
                Height = 1944,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1968,
                LineTimeNs = 32503,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode3Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode3Regs.Count,
                Width = 2592,
                Height = 1944,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1968,
                LineTimeNs = 183789,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode4Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode4Regs.Count,
                Width = 1296,
                Height = 976,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 996,
                LineTimeNs = 23216,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode5Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode5Regs.Count,
                Width = 1296,
                Height = 730,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 754,
                LineTimeNs = 23216,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode6Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode6Regs.Count,
                Width = 640,
                Height = 480,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 484,
                LineTimeNs = 31749,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            },
            new ModeDef
            {
                Regs = Ov5647SensorRegs.Ov5647Mode7Regs,
                NumRegs = Ov5647SensorRegs.Ov5647Mode7Regs.Count,
                Width = 640,
                Height = 480,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_GBRG,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 484,
                LineTimeNs = 21165,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 16
            }
        };     
    }
}
