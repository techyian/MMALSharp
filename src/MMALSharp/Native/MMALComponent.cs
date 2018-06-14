// <copyright file="MMALComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
        public IntPtr Priv { get; }

        public IntPtr UserData { get; }

        public char* Name { get; }

        public uint IsEnabled { get; }

        public MMAL_PORT_T* Control { get; }

        public uint InputNum { get; }

        public MMAL_PORT_T** Input { get; }

        public uint OutputNum { get; }

        public MMAL_PORT_T** Output { get; }

        public uint ClockNum { get; }

        public MMAL_PORT_T** Clock { get; }

        public uint PortNum { get; }

        public MMAL_PORT_T** Port { get; }

        public uint Id { get; }

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
            this.Priv = priv;
            this.UserData = userData;
            this.Name = name;
            this.IsEnabled = isEnabled;
            this.Control = control;
            this.InputNum = inputNum;
            this.Input = input;
            this.OutputNum = outputNum;
            this.Output = output;
            this.ClockNum = clockNum;
            this.Clock = clock;
            this.PortNum = portNum;
            this.Port = port;
            this.Id = id;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_WRAPPER_T
    {
        private MMAL_COMPONENT_T* component;

        public IntPtr UserData { get; }

        public IntPtr Callback { get; }

        public MMALUtil.MMAL_STATUS_T Status { get; }

        public MMAL_PORT_T* Control { get; }

        public uint InputNum { get; }

        public MMAL_PORT_T** Input { get; }

        public MMAL_POOL_T** InputPool { get; }

        public uint OutputNum { get; }

        public MMAL_PORT_T** Output { get; }

        public MMAL_POOL_T** OutputPool { get; }

        public MMAL_QUEUE_T** OutputQueue { get; }

        public long TimeSetup { get; }

        public long TimeEnable { get; }

        public long TimeDisable { get; }

        public MMAL_WRAPPER_T (
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
            this.UserData = userData;
            this.Callback = callback;
            this.component = component;
            this.Status = status;
            this.Control = control;
            this.InputNum = inputNum;
            this.Input = input;
            this.InputPool = inputPool;
            this.OutputNum = outputNum;
            this.Output = output;
            this.OutputPool = outputPool;
            this.OutputQueue = outputQueue;
            this.TimeSetup = timeSetup;
            this.TimeEnable = timeEnable;
            this.TimeDisable = timeDisable;
        }
    }
}
