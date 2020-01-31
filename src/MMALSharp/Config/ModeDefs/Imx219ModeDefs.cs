using MMALSharp.Config.SensorRegs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMALSharp.Config.ModeDefs
{
    public static class Imx219ModeDefs
    {
        public static readonly List<ModeDef> ModeDefs = new List<ModeDef>
        {
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Regs,
                NumRegs = Imx219SensorRegs.Imx219Regs.Count,
                Width = 3280,
                Height = 2464,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 2504,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode1Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode1Regs.Count,
                Width = 1920,
                Height = 1080,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1084,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode2Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode2Regs.Count,
                Width = 3280,
                Height = 2464,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 2468,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode3Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode3Regs.Count,
                Width = 3280,
                Height = 2464,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 2468,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode4Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode4Regs.Count,
                Width = 1640,
                Height = 1232,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 1236,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode5Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode5Regs.Count,
                Width = 1640,
                Height = 922,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 926,
                LineTimeNs = 18904,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode6Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode6Regs.Count,
                Width = 1280,
                Height = 720,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 724,
                LineTimeNs = 19517,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            },
            new ModeDef
            {
                Regs = Imx219SensorRegs.Imx219Mode7Regs,
                NumRegs = Imx219SensorRegs.Imx219Mode7Regs.Count,
                Width = 640,
                Height = 480,
                Encoding = 0,
                Order = BayerOrder.BAYER_ORDER_BGGR,
                NativeBitDepth = 10,
                ImageId = 0x2B,
                DataLanes = 2,
                MinVts = 484,
                LineTimeNs = 19517,
                Timing = new int[] { 0, 0, 0, 0 },
                Term = new int[] { 0, 0 },
                BlackLevel = 66
            }
        };
    }
}
