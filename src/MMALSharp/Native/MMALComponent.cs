// <copyright file="MMALComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line

    public static class MMALComponent
    {
        // name: char* * comp: MMAL_COMPONENT_T** 
#pragma warning disable IDE1006 // Naming Styles
        [DllImport("libmmal.so", EntryPoint = "mmal_component_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_component_create(string name, IntPtr* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_acquire", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_component_acquire(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_component_release(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_component_destroy(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_enable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_component_enable(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_disable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_component_disable(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_wrapper_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_wrapper_create(IntPtr* wrapper, string name);

        [DllImport("libmmal.so", EntryPoint = "mmal_wrapper_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_wrapper_destroy(IntPtr* wrapper);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_COMPONENT_PRIVATE_T
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_COMPONENT_T
    {
        private IntPtr priv, userData;
        private char* name;
        private uint isEnabled;
        private MMAL_PORT_T* control;
        private uint inputNum;
        private MMAL_PORT_T** input;
        private uint outputNum;
        private MMAL_PORT_T** output;
        private uint clockNum;
        private MMAL_PORT_T** clock;
        private uint portNum;
        private MMAL_PORT_T** port;
        private uint id;

        public IntPtr Priv => this.priv;

        public IntPtr UserData => this.userData;

        public char* Name => this.name;

        public uint IsEnabled => this.isEnabled;

        public MMAL_PORT_T* Control => this.control;

        public uint InputNum => this.inputNum;

        public MMAL_PORT_T** Input => this.input;

        public uint OutputNum => this.outputNum;

        public MMAL_PORT_T** Output => this.output;

        public uint ClockNum => this.clockNum;

        public MMAL_PORT_T** Clock => this.clock;

        public uint PortNum => this.portNum;

        public MMAL_PORT_T** Port => this.port;

        public uint Id => this.id;

        public MMAL_COMPONENT_T(
                                IntPtr priv,
                                IntPtr userData,
                                char* name,
                                uint isEnabled,
                                MMAL_PORT_T* control,
                                uint inputNum,
                                MMAL_PORT_T** input,
                                uint outputNum,
                                MMAL_PORT_T** output,
                                uint clockNum,
                                MMAL_PORT_T** clock,
                                uint portNum,
                                MMAL_PORT_T** port,
                                uint id)
        {
            this.priv = priv;
            this.userData = userData;
            this.name = name;
            this.isEnabled = isEnabled;
            this.control = control;
            this.inputNum = inputNum;
            this.input = input;
            this.outputNum = outputNum;
            this.output = output;
            this.clockNum = clockNum;
            this.clock = clock;
            this.portNum = portNum;
            this.port = port;
            this.id = id;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_WRAPPER_T
    {
        private IntPtr userData, callback;
        private MMAL_COMPONENT_T* component;
        private MMALUtil.MMAL_STATUS_T status;
        private MMAL_PORT_T* control;
        private uint inputNum;
        private MMAL_PORT_T** input;
        private MMAL_POOL_T** inputPool;
        private uint outputNum;
        private MMAL_PORT_T** output;
        private MMAL_POOL_T** outputPool;
        private MMAL_QUEUE_T** outputQueue;
        private long timeSetup, timeEnable, timeDisable;

        public IntPtr UserData => this.userData;

        public IntPtr Callback => this.callback;

        public MMALUtil.MMAL_STATUS_T Status => this.status;

        public MMAL_PORT_T* Control => this.control;

        public uint InputNum => this.inputNum;

        public MMAL_PORT_T** Input => this.input;

        public MMAL_POOL_T** InputPool => this.inputPool;

        public uint OutputNum => this.outputNum;

        public MMAL_PORT_T** Output => this.output;

        public MMAL_POOL_T** OutputPool => this.outputPool;

        public MMAL_QUEUE_T** OutputQueue => this.outputQueue;

        public long TimeSetup => this.timeSetup;

        public long TimeEnable => this.timeEnable;

        public long TimeDisable => this.timeDisable;

        public MMAL_WRAPPER_T(
                               IntPtr userData,
                               IntPtr callback,
                               MMAL_COMPONENT_T* component,
                               MMALUtil.MMAL_STATUS_T status,
                               MMAL_PORT_T* control,
                               uint inputNum,
                               MMAL_PORT_T** input,
                               MMAL_POOL_T** inputPool,
                               uint outputNum,
                               MMAL_PORT_T** output,
                               MMAL_POOL_T** outputPool,
                               MMAL_QUEUE_T** outputQueue,
                               long timeSetup,
                               long timeEnable,
                               long timeDisable)
        {
            this.userData = userData;
            this.callback = callback;
            this.component = component;
            this.status = status;
            this.control = control;
            this.inputNum = inputNum;
            this.input = input;
            this.inputPool = inputPool;
            this.outputNum = outputNum;
            this.output = output;
            this.outputPool = outputPool;
            this.outputQueue = outputQueue;
            this.timeSetup = timeSetup;
            this.timeEnable = timeEnable;
            this.timeDisable = timeDisable;
        }
    }
}
