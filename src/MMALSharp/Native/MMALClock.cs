// <copyright file="MMALClock.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALClock
    {
        public static int MMAL_CLOCK_EVENT_MAGIC = MMALUtil.MMAL_FOURCC("CKLM");
        public static int MMAL_CLOCK_EVENT_REFERENCE = MMALUtil.MMAL_FOURCC("CREF");
        public static int MMAL_CLOCK_EVENT_ACTIVE = MMALUtil.MMAL_FOURCC("CACT");
        public static int MMAL_CLOCK_EVENT_SCALE = MMALUtil.MMAL_FOURCC("CSCA");
        public static int MMAL_CLOCK_EVENT_TIME = MMALUtil.MMAL_FOURCC("CTIM");
        public static int MMAL_CLOCK_EVENT_UPDATE_THRESHOLD = MMALUtil.MMAL_FOURCC("CUTH");
        public static int MMAL_CLOCK_EVENT_DISCONT_THRESHOLD = MMALUtil.MMAL_FOURCC("CDTH");
        public static int MMAL_CLOCK_EVENT_REQUEST_THRESHOLD = MMALUtil.MMAL_FOURCC("CRTH");
        public static int MMAL_CLOCK_EVENT_INPUT_BUFFER_INFO = MMALUtil.MMAL_FOURCC("CIBI");
        public static int MMAL_CLOCK_EVENT_OUTPUT_BUFFER_INFO = MMALUtil.MMAL_FOURCC("COBI");
        public static int MMAL_CLOCK_EVENT_LATENCY = MMALUtil.MMAL_FOURCC("CLAT");
        public static int MMAL_CLOCK_EVENT_INVALID = 0;                
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_UPDATE_THRESHOLD_T
    {
        public long ThresholdLower { get; }

        public long ThresholdUpper { get; }

        public MMAL_CLOCK_UPDATE_THRESHOLD_T(long thresholdLower, long thresholdUpper)
        {
            this.ThresholdLower = thresholdLower;
            this.ThresholdUpper = thresholdUpper;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_DISCONT_THRESHOLD_T
    {
        public long Threshold { get; }

        public long Duration { get; }

        public MMAL_CLOCK_DISCONT_THRESHOLD_T(long threshold, long duration)
        {
            this.Threshold = threshold;
            this.Duration = duration;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_REQUEST_THRESHOLD_T
    {
        public long Threshold { get; }

        public int ThresholdEnable { get; }

        public MMAL_CLOCK_REQUEST_THRESHOLD_T(long threshold, int thresholdEnable)
        {
            this.Threshold = threshold;
            this.ThresholdEnable = thresholdEnable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_BUFFER_INFO_T
    {
        public long Timestamp { get; }

        public uint ArrivalTime { get; }

        public MMAL_CLOCK_BUFFER_INFO_T(long timestamp, uint arrivalTime)
        {
            this.Timestamp = timestamp;
            this.ArrivalTime = arrivalTime;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_LATENCY_T
    {
        public long Target { get; }

        public long AttackPeriod { get; }

        public long AttackRate { get; }

        public MMAL_CLOCK_LATENCY_T(long target, long attackPeriod, long attackRate)
        {
            this.Target = target;
            this.AttackPeriod = attackPeriod;
            this.AttackRate = attackRate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CLOCK_EVENT_DATA
    {
        public int Enable { get; }

        public MMAL_RATIONAL_T Scale { get; }

        public MMAL_CLOCK_UPDATE_THRESHOLD_T UpdateThreshold { get; }

        public MMAL_CLOCK_DISCONT_THRESHOLD_T DiscontThreshold { get; }

        public MMAL_CLOCK_REQUEST_THRESHOLD_T RequestThreshold { get; }

        public MMAL_CLOCK_BUFFER_INFO_T Buffer { get; }

        public MMAL_CLOCK_LATENCY_T Latency { get; }

        public MMAL_CLOCK_EVENT_DATA(int enable, MMAL_RATIONAL_T scale, MMAL_CLOCK_UPDATE_THRESHOLD_T updateThreshold,
                                     MMAL_CLOCK_DISCONT_THRESHOLD_T discontThreshold, MMAL_CLOCK_REQUEST_THRESHOLD_T requestThreshold,
                                     MMAL_CLOCK_BUFFER_INFO_T buffer, MMAL_CLOCK_LATENCY_T latency)
        {
            this.Enable = enable;
            this.Scale = scale;
            this.UpdateThreshold = updateThreshold;
            this.DiscontThreshold = discontThreshold;
            this.RequestThreshold = requestThreshold;
            this.Buffer = buffer;
            this.Latency = latency;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_CLOCK_EVENT_T
    {
        public uint Id { get; }

        public uint Magic { get; }

        public MMAL_BUFFER_HEADER_T* Buffer { get; }

        public uint Padding0 { get; }

        public MMAL_CLOCK_EVENT_DATA Data { get; }

        public long Padding1 { get; }

        public MMAL_CLOCK_EVENT_T(uint id, uint magic, MMAL_BUFFER_HEADER_T* buffer, uint padding0,
                                  MMAL_CLOCK_EVENT_DATA data, long padding1)
        {
            this.Id = id;
            this.Magic = magic;
            this.Buffer = buffer;
            this.Padding0 = padding0;
            this.Data = data;
            this.Padding1 = padding1;
        }
    }
}
