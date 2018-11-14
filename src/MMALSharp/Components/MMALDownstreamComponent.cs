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
            MMALCamera.Instance.DownstreamComponents.Add(this);
            this.ProcessingPorts = new Dictionary<int, IOutputPort>();
        }

        /// <summary>
        /// The width that this component will try to output data in.   
        /// </summary>
        public abstract int Width { get; set; }

        /// <summary>
        /// The height that this component will try to output data in.   
        /// </summary>
        public abstract int Height { get; set; }

        /// <summary>
        /// A list of working ports which are processing data in the component pipeline.
        /// </summary>
        public Dictionary<int, IOutputPort> ProcessingPorts { get; set; }
        
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
        public void RemoveConnectionCallback(IPort port) =>
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
        public virtual MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, IPort copyPort, bool zeroCopy = false)
        {
            this.InitialiseInputPort(0);

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
        /// Call to configure changes on an Image Encoder input port. Used when providing an image file directly
        /// to the component.
        /// </summary>
        /// <param name="encodingType">The encoding type the input port will expect data in.</param>
        /// <param name="pixelFormat">The pixel format the input port will expect data in.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, bool zeroCopy = false)
        {
            return this.ConfigureInputPort(encodingType, pixelFormat, 0, 0, zeroCopy);
        }

        /// <summary>
        /// Call to configure changes on an Image Encoder input port. Used when providing an image file directly
        /// to the component.
        /// </summary>
        /// <param name="encodingType">The encoding type the input port will expect data in.</param>
        /// <param name="pixelFormat">The pixel format the input port will expect data in.</param>
        /// <param name="width">The width of the incoming frame.</param>
        /// <param name="height">The height of the incoming frame.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height, bool zeroCopy = false)
        {
            this.InitialiseInputPort(0);

            if (encodingType != null)
            {
                this.Inputs[0].NativeEncodingType = encodingType.EncodingVal;
            }

            if (pixelFormat != null)
            {
                this.Inputs[0].NativeEncodingSubformat = pixelFormat.EncodingVal;
            }

            this.Inputs[0].BufferNum = this.Inputs[0].Ptr->BufferNumMin;
            this.Inputs[0].BufferSize = this.Inputs[0].Ptr->BufferSizeMin;

            this.Inputs[0].EncodingType = encodingType;

            this.Inputs[0].Commit();

            if (this.Outputs.Count > 0 && this.Outputs[0].Ptr->Format->Type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            if (zeroCopy)
            {
                this.Inputs[0].ZeroCopy = true;
                this.Inputs[0].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            return this;
        }

        /// <summary>
        /// Call to configure changes on the 1st Downstream component output port.
        /// </summary>
        /// <param name="encodingType">The encoding type this output port will send data in.</param>
        /// <param name="pixelFormat">The pixel format this output port will send data in.</param>
        /// <param name="quality">The quality of our outputted data.</param>
        /// <param name="bitrate">The bitrate we are sending data at.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual MMALDownstreamComponent ConfigureOutputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0, bool zeroCopy = false)
        {
            return this.ConfigureOutputPort(0, encodingType, pixelFormat, quality, bitrate, zeroCopy);
        }
        
        /// <summary>
        /// Call to configure changes on an Downstream component output port.
        /// </summary>
        /// <param name="outputPort">The output port we are configuring.</param>
        /// <param name="encodingType">The encoding type this output port will send data in.</param>
        /// <param name="pixelFormat">The pixel format this output port will send data in.</param>
        /// <param name="quality">The quality of our outputted data.</param>
        /// <param name="bitrate">The bitrate we are sending data at.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <returns>This <see cref="MMALDownstreamComponent"/>.</returns>
        public virtual unsafe MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0, bool zeroCopy = false)
        {
            this.InitialiseOutputPort(outputPort);

            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);
            
            this.Inputs[0].ShallowCopy(this.Outputs[outputPort]);

            if (encodingType != null)
            {
                this.Outputs[outputPort].NativeEncodingType = encodingType.EncodingVal;
            }

            if (pixelFormat != null)
            {
                this.Outputs[outputPort].NativeEncodingSubformat = pixelFormat.EncodingVal;
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

            if (encodingType == MMALEncoding.JPEG)
            {
                this.Outputs[outputPort].SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, quality);
            }

            if (zeroCopy)
            {
                this.Outputs[outputPort].ZeroCopy = true;
                this.Outputs[outputPort].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            this.Outputs[outputPort].EncodingType = encodingType;
            this.Outputs[outputPort].PixelFormat = pixelFormat;

            this.Outputs[outputPort].Resolution = new Resolution(this.Width, this.Height).Pad();
            this.Outputs[outputPort].Crop = new Rectangle(0, 0, this.Width, this.Height);
                        
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

            MMALCamera.Instance.DownstreamComponents.Remove(this);

            base.Dispose();
        }

        /// <summary>
        /// Initialises the input port specified by constructing the correct port type to be used by the component.
        /// </summary>
        /// <param name="inputPort">The input port to initialise.</param>
        internal virtual void InitialiseInputPort(int inputPort)
        {
        }

        /// <summary>
        /// Initialises the output port specified by constructing the correct port type to be used by the component.
        /// </summary>
        /// <param name="outputPort">The output port to initialise.</param>
        internal virtual void InitialiseOutputPort(int outputPort)
        {
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
