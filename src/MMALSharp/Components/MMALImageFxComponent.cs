// <copyright file="MMALImageFxComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Common.Utility;
using MMALSharp.Config;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.MMALNativeExceptionHelper;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp.Components
{
    /// <summary>
    /// The ImageFx component is used to apply image effects to a YUV_UV image or video frame. YUV420 packed planar and YUV422 packed planar image formats are supported.
    /// </summary>
    public class MMALImageFxComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Query / Set the current image effect.
        /// </summary>
        public MMAL_PARAM_IMAGEFX_T ImageEffect
        {
            get => this.GetCurrentImageEffect();
            set => this.SetCurrentImageEffect(value);
        }

        /// <summary>
        /// Query / Set the colour enhancement. This overwrites the U/V planes with a constant value, and is applied after any currently selected filter. 
        /// </summary>
        public ColourEffects ColourEnhancement
        {
            get => this.GetColourEnhancementValue();
            set => this.SetColourEnhancementValue(value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFxComponent"/>.
        /// </summary>
        public unsafe MMALImageFxComponent()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_FX)
        {
            // Default to use still image port behaviour.
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));
            this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[0])), this, Guid.NewGuid()));
        }
        
        private unsafe ColourEffects GetColourEnhancementValue()
        {
            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0,
                                                                                                        0,
                                                                                                        0);

            MMALCheck(MMALPort.mmal_port_parameter_get(this.Outputs[0].Ptr, &colFx.Hdr), "Unable to get colour enhancement value.");

            ColourEffects fx = new ColourEffects(colFx.Enable == 1, MMALColor.FromYUVBytes(0, (byte)colFx.U, (byte)colFx.V));

            return fx;
        }

        private unsafe void SetColourEnhancementValue(ColourEffects colourFx)
        {
            var uv = MMALColor.RGBToYUVBytes(colourFx.Color);

            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        colourFx.Enable ? 1 : 0,
                                                                                                        uv.Item2,
                                                                                                        uv.Item3);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[0].Ptr, &colFx.Hdr), "Unable to set colour enhancement value.");
        }

        private unsafe MMAL_PARAM_IMAGEFX_T GetCurrentImageEffect()
        {
            MMAL_PARAM_IMAGEFX_T value = default;

            MMAL_PARAMETER_IMAGEFX_T effectStr = new MMAL_PARAMETER_IMAGEFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                value);

            MMALCheck(MMALPort.mmal_port_parameter_get(this.Outputs[0].Ptr, &effectStr.Hdr), "Unable to get current effect value.");

            return value;
        }

        private unsafe void SetCurrentImageEffect(MMAL_PARAM_IMAGEFX_T effect)
        {
            MMAL_PARAMETER_IMAGEFX_T effectStr = new MMAL_PARAMETER_IMAGEFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                effect);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[0].Ptr, &effectStr.Hdr), "Unable to set current effect value.");
        }
    }
}
