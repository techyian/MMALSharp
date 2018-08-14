// <copyright file="MMALRendererComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all renderer components.
    /// </summary>
    public abstract class MMALRendererBase : MMALDownstreamComponent
    {
        /// <summary>
        /// Create a new instance of a renderer component.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        protected MMALRendererBase(string name)
            : base(name)
        {
        }
    }

    /// <summary>
    /// Represents a Null Sink component. This component should be used when a preview component is not required in order to measure exposure.
    /// </summary>
    public class MMALNullSinkComponent : MMALRendererBase
    {
        private int _width;
        private int _height;

        /// <summary>
        /// Gets or sets the width of resulting frames. This property has no direct effect as no frames will exit a Null Sink component.
        /// If not specified, the camera's video resolution is used.
        /// </summary>
        public override int Width
        {
            get => _width == 0 ? MMALCameraConfig.VideoResolution.Width : _width;
            set => _width = value;
        }

        /// <summary>
        /// Gets or sets the height of resulting frames. This property has no direct effect as no frames will exit a Null Sink component.
        /// If not specified, the camera's video resolution is used.
        /// </summary>
        public override int Height
        {
            get => _height == 0 ? MMALCameraConfig.VideoResolution.Height : _height;
            set => _height = value;
        }

        /// <summary>
        /// Creates a new instance of a Null Sink renderer component. This component is intended to be connected to the Camera's preview port
        /// and is used to measure exposure. No video preview is available with this renderer.
        /// </summary>
        public MMALNullSinkComponent()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_NULL_SINK)
        {
            this.EnableComponent();
        }

        /// <summary>
        /// Prints the name of this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Null sink renderer");
        }
    }

    /// <summary>
    /// Represents a Video Renderer component.
    /// </summary>
    public class MMALVideoRenderer : MMALRendererBase
    {
        private int _width;
        private int _height;

        /// <summary>
        /// Gets or sets the width of resulting frames. If not specified, the camera's video resolution is used.
        /// </summary>
        public override int Width
        {
            get => _width == 0 ? MMALCameraConfig.VideoResolution.Width : _width;
            set => _width = value;
        }

        /// <summary>
        /// Gets or sets the height of resulting frames. If not specified, the camera's video resolution is used.
        /// </summary>
        public override int Height
        {
            get => _height == 0 ? MMALCameraConfig.VideoResolution.Height : _height;
            set => _height = value;
        }

        /// <summary>
        /// Gets the configuration for this video renderer. Call <see cref="ConfigureRenderer"/> to apply changes.
        /// </summary>
        public PreviewConfiguration Configuration { get; }

        /// <summary>
        /// Gets a list of overlay renderers connected to this video renderer.
        /// </summary>
        public List<MMALOverlayRenderer> Overlays { get; } = new List<MMALOverlayRenderer>();

        /// <summary>
        /// Creates a new instance of a Video renderer component. This component is intended to be connected to the Camera's preview port
        /// and is used to measure exposure. It also produces real-time video to the Pi's HDMI output from the camera.
        /// </summary>
        public MMALVideoRenderer()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
            this.EnableComponent();
        }

        /// <summary>
        /// Creates a new instance of a Video renderer component. This component is intended to be connected to the Camera's preview port
        /// and is used to measure exposure. It also produces real-time video to the Pi's HDMI output from the camera.
        /// </summary>
        /// <param name="config">The configuration object for this renderer.</param>
        public MMALVideoRenderer(PreviewConfiguration config)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
            this.EnableComponent();
            this.Configuration = config;
        }

        /// <summary>
        /// Removes a <see cref="MMALOverlayRenderer"/> from this renderer's overlays if it exists.
        /// </summary>
        /// <param name="renderer">The overlay renderer to remove.</param>
        public void RemoveOverlay(MMALOverlayRenderer renderer)
        {
            this.Overlays.Remove(renderer);
            renderer.Dispose();
        }
            
        /// <summary>
        /// Commits all changes made to the configuration.
        /// </summary>
        /// <exception cref="MMALException"/>
        public unsafe void ConfigureRenderer()
        {
            if (this.Configuration != null)
            {
                int fullScreen = 0, noAspect = 0, copyProtect = 0;
                uint displaySet = 0;

                MMAL_RECT_T? previewWindow = new MMAL_RECT_T?();

                if (!this.Configuration.FullScreen)
                {
                    previewWindow = new MMAL_RECT_T(
                        this.Configuration.PreviewWindow.X, this.Configuration.PreviewWindow.Y,
                        this.Configuration.PreviewWindow.Width, this.Configuration.PreviewWindow.Height);
                }

                displaySet = (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_LAYER;
                displaySet |= (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_ALPHA;

                if (this.Configuration.FullScreen)
                {
                    displaySet |= (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_FULLSCREEN;
                    fullScreen = 1;
                }
                else
                {
                    displaySet |= (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_DEST_RECT |
                                  (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_FULLSCREEN;
                }

                if (this.Configuration.NoAspect)
                {
                    displaySet |= (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_NOASPECT;
                    noAspect = 1;
                }

                if (this.Configuration.CopyProtect)
                {
                    displaySet |= (int)MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_COPYPROTECT;
                    copyProtect = 1;
                }

                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_DISPLAYREGION_T>());

                MMAL_DISPLAYREGION_T displayRegion = new MMAL_DISPLAYREGION_T(
                    new MMAL_PARAMETER_HEADER_T(
                        MMALParametersVideo.MMAL_PARAMETER_DISPLAYREGION,
                        Marshal.SizeOf<MMAL_DISPLAYREGION_T>()), displaySet, 0, fullScreen, this.Configuration.DisplayTransform, previewWindow ?? new MMAL_RECT_T(0, 0, 0, 0), new MMAL_RECT_T(0, 0, 0, 0), noAspect,
                        this.Configuration.DisplayMode, 0, 0, this.Configuration.Layer, copyProtect, this.Configuration.Opacity);

                Marshal.StructureToPtr(displayRegion, ptr, false);

                try
                {
                    MMALCheck(MMALPort.mmal_port_parameter_set(this.Inputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), $"Unable to set preview renderer configuration");
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        /// <summary>
        /// Prints the name of this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Video renderer");
        }

        /// <summary>
        /// Disposes the component, and frees any native resources still in use by it.
        /// </summary>
        public override void Dispose()
        {
            if (this.GetType() == typeof(MMALVideoRenderer))
            {
                Overlays.ForEach(c => c.Dispose());
            }
            
            base.Dispose();
        }
    }

    /// <summary>
    /// MMAL provides the ability to add a static video render overlay onto the display output. The user must provide unencoded RGB input padded to the width/height of the camera block size (32x16).
    /// This class represents a video renderer which has the ability to overlay static resources to the display output.
    /// </summary>
    public sealed class MMALOverlayRenderer : MMALVideoRenderer
    {
        /// <summary>
        /// A reference to the current stream being used in the overlay.
        /// </summary>
        public byte[] Source { get; set; }

        /// <summary>
        /// The parent renderer which is being used to overlay onto the display.
        /// </summary>
        public MMALVideoRenderer ParentRenderer { get; set; }

        /// <summary>
        /// The configuration for rendering a static preview overlay.
        /// </summary>
        public PreviewOverlayConfiguration OverlayConfiguration { get; set; }

        /// <summary>
        /// A list of supported encodings for overlay image data.
        /// </summary>
        public readonly IReadOnlyCollection<MMALEncoding> AllowedEncodings = new ReadOnlyCollection<MMALEncoding>(new List<MMALEncoding>
        {
            MMALEncoding.I420,
            MMALEncoding.RGB24,
            MMALEncoding.RGBA,
            MMALEncoding.BGR24,
            MMALEncoding.BGRA
        });

        /// <summary>
        /// Creates a new instance of a Overlay renderer component. This component is identical to the <see cref="MMALVideoRenderer"/> class, however it provides
        /// the ability to overlay a static source onto the render overlay.
        /// </summary>
        /// <param name="parent">The parent renderer which is being used to overlay onto the display.</param>
        /// <param name="config">The configuration for rendering a static preview overlay.</param>
        /// <param name="source">A reference to the current stream being used in the overlay.</param>
        public MMALOverlayRenderer(MMALVideoRenderer parent, PreviewOverlayConfiguration config, byte[] source)
            : base(config)
        {
            this.Source = source;
            this.ParentRenderer = parent;
            this.OverlayConfiguration = config;
            parent.Overlays.Add(this);
            
            this.Inputs[0] = new MMALOverlayPort(this.Inputs[0]);
            
            if (config != null)
            {
                if (config.Resolution.Width > 0 && config.Resolution.Height > 0)
                {
                    this.Inputs[0].Resolution = config.Resolution;
                    this.Inputs[0].Crop = new Rectangle(0, 0, config.Resolution.Width, config.Resolution.Height);
                }
                else
                {
                    this.Inputs[0].Resolution = parent.Inputs[0].Resolution;
                    this.Inputs[0].Crop = new Rectangle(0, 0, parent.Inputs[0].Resolution.Width, parent.Inputs[0].Resolution.Height);
                }

                this.Inputs[0].FrameRate = new MMAL_RATIONAL_T(0, 0);

                if (config.Encoding == null)
                {
                    var sourceLength = source.Length;
                    var planeSize = this.Inputs[0].Resolution.Pad();
                    var planeLength = Math.Floor((double)planeSize.Width * planeSize.Height);

                    if (Math.Floor(sourceLength / planeLength) == 3)
                    {
                        config.Encoding = MMALEncoding.RGB24;
                    }
                    else if (Math.Floor(sourceLength / planeLength) == 4)
                    {
                        config.Encoding = MMALEncoding.RGBA;
                    }
                    else
                    {
                        throw new PiCameraError("Unable to determine encoding from image size.");
                    }
                }

                this.Inputs[0].NativeEncodingType = config.Encoding.EncodingVal;
            }

            if (!this.AllowedEncodings.Any(c => c.EncodingVal == this.Inputs[0].NativeEncodingType))
            {
                throw new PiCameraError($"Incompatible encoding type for use with Preview Render overlay {MMALEncodingHelpers.ParseEncoding(this.Inputs[0].NativeEncodingType).EncodingName}.");
            }

            this.Inputs[0].Commit();
            
            this.Start(this.Control);
            this.Start(this.Inputs[0]);
        }

        /// <summary>
        /// Updates the overlay by sending <see cref="Source"/> as new image data.
        /// </summary>
        public void UpdateOverlay()
        {
            this.UpdateOverlay(this.Source);
        }

        /// <summary>
        /// Updates the overlay by sending the specified buffer as new image data.
        /// </summary>
        /// <param name="imageData">Byte array containing the image data encoded like configured.</param>
        public void UpdateOverlay(byte[] imageData)
        {
            lock (MMALPortBase.InputLock)
            {
                var buffer = this.Inputs[0].BufferPool.Queue.GetBuffer();

                if (buffer == null)
                {
                    MMALLog.Logger.Warn("Received null buffer when updating overlay.");
                    return;
                }
                
                buffer.ReadIntoBuffer(imageData, imageData.Length, false);
                this.Inputs[0].SendBuffer(buffer);
            }
        }
    }
}