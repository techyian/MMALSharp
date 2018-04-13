// <copyright file="MMALRendererComponent.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using System.Runtime.InteropServices;
using MMALSharp.Native;
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

        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.VideoResolution.Width;
                }
                return _width;
            }
            set => _width = value;
        }

        public override int Height
        {
            get
            {
                if (_height == 0)
                {
                    return MMALCameraConfig.VideoResolution.Height;
                }
                return _height;
            }
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
        
        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.VideoResolution.Width;
                }
                return _width;
            }
            set => _width = value;
        }

        public override int Height
        {
            get
            {
                if (_height == 0)
                {
                    return MMALCameraConfig.VideoResolution.Height;
                }
                return _height;
            }
            set => _height = value;
        }

        public PreviewConfiguration Configuration { get; }

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
        public MMALVideoRenderer(PreviewConfiguration config)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
            this.EnableComponent();
            this.Configuration = config;
        }

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
                
                displaySet = (int) MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_LAYER;
                displaySet |= (int) MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_ALPHA;
                
                if (this.Configuration.FullScreen)
                {
                    displaySet |= (int) MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_FULLSCREEN;
                    fullScreen = 1;
                }
                else
                {
                    displaySet |= ((int) MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_DEST_RECT |
                                    (int) MMALParametersVideo.MMAL_DISPLAYSET_T.MMAL_DISPLAY_SET_FULLSCREEN);
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

        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Video renderer");
        }
    }

    public class MMALOverlayRenderer
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
        /// Creates a new instance of a Overlay renderer component. This component is identical to the <see cref="MMALVideoRenderer"/> class, however it provides
        /// the ability to overlay a static source onto the render overlay.
        /// </summary>
        public MMALOverlayRenderer(MMALVideoRenderer parent, byte[] source)
        {
            this.Source = source;
            this.ParentRenderer = parent;
        }
        
        public void UpdateOverlay(FileStream stream)
        {
            var buffer = this.ParentRenderer.Inputs[0].BufferPool.Queue.GetBuffer();
            buffer.ReadIntoBuffer(this.Source, this.Source.Length, false);
            this.ParentRenderer.Inputs[0].SendBuffer(buffer);
        }
    }
}
