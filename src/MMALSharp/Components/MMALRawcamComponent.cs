// <copyright file="MMALRawcamComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Ports.Inputs;
using MMALSharp.Native;
using System;
using MMALSharp.Ports.Outputs;
using MMALSharp.Ports;
using MMALSharp.Handlers;
using System.Runtime.InteropServices;
using MMALSharp.Common.Utility;
using Microsoft.Extensions.Logging;
using static MMALSharp.MMALNativeExceptionHelper;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// ----------------------------------------------------------------------------------------
    /// PLEASE NOTE: THIS IS AN EXPERIMENTAL COMPONENT AND IS NOT BASED ON PRODUCTION READY CODE - https://www.raspberrypi.org/forums/viewtopic.php?f=43&t=109137.
    /// ----------------------------------------------------------------------------------------
    /// This component interfaces directly to the camera receiver peripheral to
    /// pass the image data and metadata that is received over CSI, CCP2, or CPI.
    /// It does not use the ISP or the VPU to perform any form of processing on the image.
    /// There are a few options within the peripheral for converting between Bayer
    /// bit depths, and packing/unpacking DPCM data - that functionality is exposed.
    /// https://github.com/raspberrypi/firmware/blob/master/documentation/ilcomponents/rawcam.html    
    /// </summary>
    public class MMALRawcamComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALRawcamComponent"/> class.
        /// </summary>
        public unsafe MMALRawcamComponent()
            : base(MMAL_COMPONENT_RAWCAM)
        {
            // Default to use still image port behaviour.
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));
            this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[0])), this, Guid.NewGuid()));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MMALRawcamComponent"/> class.
        /// </summary>
        /// <param name="outputPortType">The user defined output port type.</param>
        public unsafe MMALRawcamComponent(Type outputPortType)
            : base(MMAL_COMPONENT_RAWCAM)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));
            this.Outputs.Add((IOutputPort)Activator.CreateInstance(outputPortType, (IntPtr)(&(*this.Ptr->Output[0])), this, Guid.NewGuid()));
        }

        /// <inheritdoc />
        public override IDownstreamComponent ConfigureOutputPort(int outputPort, IMMALPortConfig config, IOutputCaptureHandler handler)
        {            
            if (config is MMALRawcamPortConfig)
            {
                var rawcamConfig = config as MMALRawcamPortConfig;

                this.ConfigureCameraInterface(rawcamConfig.CameraInterface);
                this.ConfigureCameraClockingMode(rawcamConfig.ClockingMode);
                this.ConfigureCameraRxConfig(rawcamConfig.RxConfig);
                this.ConfigureTimingRegisters(rawcamConfig.TimingConfig);
            }
            else
            {
                MMALLog.Logger.LogWarning($"Rawcam component should be given port configuration of type {nameof(MMALRawcamPortConfig)}. Defaults will be used.");
            }
                        
            return base.ConfigureOutputPort(outputPort, config, handler);
        }

        private unsafe void ConfigureCameraInterface(MMAL_CAMERA_INTERFACE_T cameraInterface)
        {
            MMAL_PARAMETER_CAMERA_INTERFACE_T param = new MMAL_PARAMETER_CAMERA_INTERFACE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_INTERFACE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_INTERFACE_T>()), cameraInterface);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            try
            {
                MMALCheck(
                    MMALPort.mmal_port_parameter_set(this.Inputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr),
                    "Unable to set camera interface type.");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private unsafe void ConfigureCameraClockingMode(MMAL_CAMERA_CLOCKING_MODE_T clockingMode)
        {
            MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T param = new MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CLOCKING_MODE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T>()), clockingMode);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            try
            {
                MMALCheck(
                    MMALPort.mmal_port_parameter_set(this.Inputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr),
                    "Unable to set camera clocking mode.");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private unsafe void ConfigureCameraRxConfig(MMALRawcamRxConfig rxConfig)
        {
            MMAL_PARAMETER_CAMERA_RX_CONFIG_T param = new MMAL_PARAMETER_CAMERA_RX_CONFIG_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_RX_CONFIG, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_RX_CONFIG_T>()), rxConfig.DecodeConfig,
                                                                                            rxConfig.EncodeConfig, rxConfig.UnpackConfig, rxConfig.PackConfig, rxConfig.DataLanes, rxConfig.EncodeBlockLength, rxConfig.EmbeddedDataLines, rxConfig.ImageId);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            try
            {
                MMALCheck(
                    MMALPort.mmal_port_parameter_set(this.Inputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr),
                    "Unable to set camera peripheral config.");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private unsafe void ConfigureTimingRegisters(MMALRawcamTimingConfig timingConfig)
        {
            MMAL_PARAMETER_CAMERA_RX_TIMING_T param = new MMAL_PARAMETER_CAMERA_RX_TIMING_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_CAMERA_RX_TIMING, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_RX_TIMING_T>()), timingConfig.Timing1,
                                                                                            timingConfig.Timing2, timingConfig.Timing3, timingConfig.Timing4, timingConfig.Timing5, timingConfig.Term1, timingConfig.Term2,
                                                                                            timingConfig.CpiTiming1, timingConfig.CpiTiming2);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            try
            {
                MMALCheck(
                    MMALPort.mmal_port_parameter_set(this.Inputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr),
                    "Unable to set camera timing registers.");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
