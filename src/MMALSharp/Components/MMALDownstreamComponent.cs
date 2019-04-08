// <copyright file="MMALDownstreamComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component. A downstream component is a component that can have data passed to it from further up the component
    /// heirarchy.
    /// </summary>
    public abstract class MMALDownstreamComponent : MMALComponentBase
    {
        /// <summary>
        /// Creates a new instance of a Downstream component.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        protected MMALDownstreamComponent(string name)
            : base(name)
        {
            MMALBootstrapper.DownstreamComponents.Add(this);
            this.ProcessingPorts = new Dictionary<int, OutputPortBase>();
        }

        /// <summary>
        /// A list of working ports which are processing data in the component pipeline.
        /// </summary>
        public Dictionary<int, OutputPortBase> ProcessingPorts { get; set; }
        
        /// <summary>
        /// Registers a <see cref="IInputCallbackHandler"/>.
        /// </summary>
        /// <param name="handler">The input handler.</param>
        public void RegisterInputCallback(IInputCallbackHandler handler) => InputCallbackProvider.RegisterCallback(handler);

        /// <summary>
        /// If it exists, removes a <see cref="IInputCallbackHandler"/> on this component's input port.
        /// </summary>
        public void RemoveInputCallback() => InputCallbackProvider.RemoveCallback(this.Inputs[0]);
        
        /// <summary>
        /// Registers a <see cref="IOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="handler">The output handler.</param>
        public void RegisterOutputCallback(IOutputCallbackHandler handler) => OutputCallbackProvider.RegisterCallback(handler);

        /// <summary>
        /// If it exists, removes a <see cref="IOutputCallbackHandler"/> on the port specified.
        /// </summary>
        /// <param name="outputPort">The output port number.</param>
        public void RemoveOutputCallback(int outputPort) =>
            OutputCallbackProvider.RemoveCallback(this.Outputs[outputPort]);

        /// <summary>
        /// Registers a <see cref="IConnectionCallbackHandler"/>.
        /// </summary>
        /// <param name="handler">The output handler.</param>
        public void RegisterConnectionCallback(IConnectionCallbackHandler handler) => ConnectionCallbackProvider.RegisterCallback(handler);

        /// <summary>
        /// If it exists, removes a <see cref="IConnectionCallbackHandler"/> on the port specified.
        /// </summary>
        /// <param name="port">The port with a created connection.</param>
        public void RemoveConnectionCallback(PortBase port) =>
            ConnectionCallbackProvider.RemoveCallback(port.ConnectedReference);
        
        /// <summary>
        /// Configures a specific input port on a downstream component. This method will perform a shallow copy of the output
        /// port it is to be connected to.
        /// </summary>
        /// <param name="encodingType">The encoding type the input port shall be expecting.</param>
        /// <param name="pixelFormat">The pixel format the input port shall be expecting.</param>
        /// <param name="copyPort">The output port we are copying format data from.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, PortBase copyPort, bool zeroCopy = false)
        {
            if (copyPort != null)
            {
                copyPort.ShallowCopy(this.Inputs[0]);
            }

            if (encodingType != null)
            {
                this.Inputs[0].NativeEncodingType = encodingType.EncodingVal;
            }

            if (pixelFormat != null)
            {
                this.Inputs[0].NativeEncodingSubformat = pixelFormat.EncodingVal;
            }

            this.Inputs[0].Commit();

            if (zeroCopy)
            {
                this.Inputs[0].ZeroCopy = true;
                this.Inputs[0].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            return this;
        }

        /// <summary>
        /// Call to configure changes on a downstream component input port.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe MMALDownstreamComponent ConfigureInputPort(MMALPortConfig config)
        {
            this.Inputs[0].PortConfig = config;

            if (config.EncodingType != null)
            {
                this.Inputs[0].NativeEncodingType = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.Inputs[0].NativeEncodingSubformat = config.PixelFormat.EncodingVal;
            }
            
            if (config.Width > 0 && config.Height > 0)
            {
                this.Inputs[0].Resolution = new Resolution(config.Width, config.Height);
            }
            else
            {
                // Use config.
                this.Inputs[0].Resolution = new Resolution(0, 0);
            }

            if (config.Framerate > 0)
            {
                this.Inputs[0].FrameRate = new MMAL_RATIONAL_T(config.Framerate, 1);
            }

            if (config.Bitrate > 0)
            {
                this.Inputs[0].Bitrate = config.Bitrate;
            }

            this.Inputs[0].EncodingType = config.EncodingType;

            this.Inputs[0].Commit();

            if (this.Outputs.Count > 0 && this.Outputs[0].Ptr->Format->Type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            if (config.ZeroCopy)
            {
                this.Inputs[0].ZeroCopy = true;
                this.Inputs[0].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            this.Inputs[0].BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumMin, this.Inputs[0].Ptr->BufferNumRecommended);
            this.Inputs[0].BufferSize = Math.Max(this.Inputs[0].Ptr->BufferSizeMin, this.Inputs[0].Ptr->BufferSizeRecommended);

            return this;
        }
        
        /// <summary>
        /// Call to configure changes on the first downstream component output port.
        /// </summary>
        /// <param name="config">User provided port configuration object.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual MMALDownstreamComponent ConfigureOutputPort(MMALPortConfig config)
        {
            return this.ConfigureOutputPort(0, config);
        }

        /// <summary>
        /// Call to configure changes on a downstream component output port.
        /// </summary>
        /// <param name="outputPort">The output port number to configure.</param>
        /// <param name="config">User provided port configuration object.</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            this.Outputs[outputPort].PortConfig = config;

            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);
            
            this.Inputs[0].ShallowCopy(this.Outputs[outputPort]);

            if (config.EncodingType != null)
            {
                this.Outputs[outputPort].NativeEncodingType = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.Outputs[outputPort].NativeEncodingSubformat = config.PixelFormat.EncodingVal;
            }

            MMAL_VIDEO_FORMAT_T tempVid = this.Outputs[outputPort].Ptr->Format->Es->Video;

            try
            {
                this.Outputs[outputPort].Commit();
            }
            catch
            {
                // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                MMALLog.Logger.Warn("Commit of output port failed. Attempting to reset values.");
                this.Outputs[outputPort].Ptr->Format->Es->Video = tempVid;
                this.Outputs[outputPort].Commit();
            }

            if (config.EncodingType == MMALEncoding.JPEG)
            {
                this.Outputs[outputPort].SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, config.Quality);
            }

            if (config.ZeroCopy)
            {
                this.Outputs[outputPort].ZeroCopy = true;
                this.Outputs[outputPort].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            if (MMALCameraConfig.VideoColorSpace != null &&
                MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
            {
                this.Outputs[outputPort].VideoColorSpace = MMALCameraConfig.VideoColorSpace;
            }

            if (config.Bitrate > 0)
            {
                this.Outputs[outputPort].Bitrate = config.Bitrate;
            }
            
            this.Outputs[outputPort].EncodingType = config.EncodingType;
            this.Outputs[outputPort].PixelFormat = config.PixelFormat;

            if (config.Width > 0 && config.Height > 0)
            {
                this.Outputs[outputPort].Resolution = new Resolution(config.Width, config.Height).Pad();
                this.Outputs[outputPort].Crop = new Rectangle(0, 0, this.Outputs[outputPort].Resolution.Width, this.Outputs[outputPort].Resolution.Height);
            }
            else
            {
                // Use config or don't set depending on port type.
                this.Outputs[outputPort].Resolution = new Resolution(0, 0);

                if (this.Outputs[outputPort].Resolution.Width > 0 && this.Outputs[outputPort].Resolution.Height > 0)
                {
                    this.Outputs[outputPort].Crop = new Rectangle(0, 0, this.Outputs[outputPort].Resolution.Width, this.Outputs[outputPort].Resolution.Height);
                }
            }

            // It is important to re-commit changes to width and height.
            this.Outputs[outputPort].Commit();

            this.Outputs[outputPort].BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumRecommended, this.Outputs[outputPort].Ptr->BufferNumMin);
            this.Outputs[outputPort].BufferSize = Math.Max(this.Outputs[outputPort].Ptr->BufferSizeRecommended, this.Outputs[outputPort].Ptr->BufferSizeMin);
            
            this.Outputs[outputPort].ManagedOutputCallback = OutputCallbackProvider.FindCallback(this.Outputs[outputPort]);

            return this;
        }

        /// <summary>
        /// Disposes of the current component, closes all connections and frees all associated unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MMALLog.Logger.Debug("Removing downstream component");

            this.ClosePipelineConnections();

            MMALBootstrapper.DownstreamComponents.Remove(this);

            MMALLog.Logger.Debug($"Remaining components in pipeline: {MMALBootstrapper.DownstreamComponents.Count}");

            base.Dispose();
        }
        
        /// <summary>
        /// Responsible for closing and destroying any connections associated with this component prior to disposing.
        /// </summary>
        private void ClosePipelineConnections()
        {
            // Close any connection held by this component
            foreach (var input in this.Inputs)
            {
                if (input.ConnectedReference != null)
                {
                    MMALLog.Logger.Debug($"Removing {input.ConnectedReference.ToString()}");

                    input.ConnectedReference.OutputPort.ConnectedReference?.Dispose();
                    input.ConnectedReference.OutputPort.ConnectedReference = null;

                    input.ConnectedReference.Dispose();
                    input.ConnectedReference = null;
                }
            }

            foreach (var output in this.Outputs)
            {
                if (output.ConnectedReference != null)
                {
                    MMALLog.Logger.Debug($"Removing {output.ConnectedReference.ToString()}");

                    output.ConnectedReference.Dispose();
                    output.ConnectedReference = null;
                }
            }
        }
    }
}
