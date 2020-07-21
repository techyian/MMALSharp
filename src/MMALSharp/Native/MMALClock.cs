// <copyright file="MMALClock.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;
using MMALSharp.Common.Utility;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line

    public static class MMALClock
    {
        public static int MMAL_CLOCK_EVENT_MAGIC = Helpers.FourCCFromString("CKLM");
        public static int MMAL_CLOCK_EVENT_REFERENCE = Helpers.FourCCFromString("CREF");
        public static int MMAL_CLOCK_EVENT_ACTIVE = Helpers.FourCCFromString("CACT");
        public static int MMAL_CLOCK_EVENT_SCALE = Helpers.FourCCFromString("CSCA");
        public static int MMAL_CLOCK_EVENT_TIME = Helpers.FourCCFromString("CTIM");
        public static int MMAL_CLOCK_EVENT_UPDATE_THRESHOLD = Helpers.FourCCFromString("CUTH");
        public static int MMAL_CLOCK_EVENT_DISCONT_THRESHOLD = Helpers.FourCCFromString("CDTH");
        public static int MMAL_CLOCK_EVENT_REQUEST_THRESHOLD = Helpers.FourCCFromString("CRTH");
        public static int MMAL_CLOCK_EVENT_INPUT_BUFFER_INFO = Helpers.FourCCFromString("CIBI");
        public static int MMAL_CLOCK_EVENT_OUTPUT_BUFFER_INFO = Helpers.FourCCFromString("COBI");
        public static int MMAL_CLOCK_EVENT_LATENCY = Helpers.FourCCFromString("CLAT");
        public static int MMAL_CLOCK_EVENT_INVALID = 0;                
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_UPDATE_THRESHOLD_T
    {
        private long thresholdLower, thresholdUpper;

        public long ThresholdLower => this.thresholdLower;

        public long ThresholdUpper => this.thresholdUpper;

        public MMAL_CLOCK_UPDATE_THRESHOLD_T(long thresholdLower, long thresholdUpper)
        {
            this.thresholdLower = thresholdLower;
            this.thresholdUpper = thresholdUpper;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_DISCONT_THRESHOLD_T
    {
        private long threshold, duration;

        public long Threshold => this.threshold;

        public long Duration => this.duration;

        public MMAL_CLOCK_DISCONT_THRESHOLD_T(long threshold, long duration)
        {
            this.threshold = threshold;
            this.duration = duration;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_REQUEST_THRESHOLD_T
    {
        private long threshold;
        private int thresholdEnable;

        public long Threshold => threshold;
        public int ThresholdEnable => thresholdEnable;

        public MMAL_CLOCK_REQUEST_THRESHOLD_T(long threshold, int thresholdEnable)
        {
            this.threshold = threshold;
            this.thresholdEnable = thresholdEnable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_BUFFER_INFO_T
    {
        private long timestamp;
        private uint arrivalTime;

        public long Timestamp => timestamp;
        public uint ArrivalTime => arrivalTime;

        public MMAL_CLOCK_BUFFER_INFO_T(long timestamp, uint arrivalTime)
        {
            this.timestamp = timestamp;
            this.arrivalTime = arrivalTime;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_LATENCY_T
    {
        private long target, attackPeriod, attackRate;

        public long Target => target;
        public long AttackPeriod => attackPeriod;
        public long AttackRate => attackRate;

        public MMAL_CLOCK_LATENCY_T(long target, long attackPeriod, long attackRate)
        {
            this.target = target;
            this.attackPeriod = attackPeriod;
            this.attackRate = attackRate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_EVENT_DATA
    {
        private int enable;
        private MMAL_RATIONAL_T scale;
        private MMAL_CLOCK_UPDATE_THRESHOLD_T updateThreshold;
        private MMAL_CLOCK_DISCONT_THRESHOLD_T discontThreshold;
        private MMAL_CLOCK_REQUEST_THRESHOLD_T requestThreshold;
        private MMAL_CLOCK_BUFFER_INFO_T buffer;
        private MMAL_CLOCK_LATENCY_T latency;

        public int Enable => enable;
        public MMAL_RATIONAL_T Scale => scale;
        public MMAL_CLOCK_UPDATE_THRESHOLD_T UpdateThreshold => updateThreshold;
        public MMAL_CLOCK_DISCONT_THRESHOLD_T DiscontThreshold => discontThreshold;
        public MMAL_CLOCK_REQUEST_THRESHOLD_T RequestThreshold => requestThreshold;
        public MMAL_CLOCK_BUFFER_INFO_T Buffer => buffer;
        public MMAL_CLOCK_LATENCY_T Latency => latency;

        public MMAL_CLOCK_EVENT_DATA(int enable, MMAL_RATIONAL_T scale, MMAL_CLOCK_UPDATE_THRESHOLD_T updateThreshold,
                                     MMAL_CLOCK_DISCONT_THRESHOLD_T discontThreshold, MMAL_CLOCK_REQUEST_THRESHOLD_T requestThreshold,
                                     MMAL_CLOCK_BUFFER_INFO_T buffer, MMAL_CLOCK_LATENCY_T latency)
        {
            this.enable = enable;
            this.scale = scale;
            this.updateThreshold = updateThreshold;
            this.discontThreshold = discontThreshold;
            this.requestThreshold = requestThreshold;
            this.buffer = buffer;
            this.latency = latency;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_CLOCK_EVENT_T
    {
        private uint id, magic;
        private MMAL_BUFFER_HEADER_T* buffer;
        private uint padding0;
        private MMAL_CLOCK_EVENT_DATA data;
        private long padding1;

        public uint Id => id;
        public uint Magic => magic;
        public MMAL_BUFFER_HEADER_T* Buffer => buffer;
        public uint Padding0 => padding0;
        public MMAL_CLOCK_EVENT_DATA Data => data;
        public long Padding1 => padding1;

        public MMAL_CLOCK_EVENT_T(uint id, uint magic, MMAL_BUFFER_HEADER_T* buffer, uint padding0,
                                  MMAL_CLOCK_EVENT_DATA data, long padding1)
        {
            this.id = id;
            this.magic = magic;
            this.buffer = buffer;
            this.padding0 = padding0;
            this.data = data;
            this.padding1 = padding1;
        }
    }
}
