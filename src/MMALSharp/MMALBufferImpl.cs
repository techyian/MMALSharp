// <copyright file="MMALBufferImpl.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a buffer header object.
    /// </summary>
    public unsafe class MMALBufferImpl : MMALObject, IBuffer
    {
        /// <summary>
        /// Pointer to the data associated with this buffer header.
        /// </summary>
        public byte* Data => this.Ptr->data;

        /// <summary>
        /// Defines what the buffer header contains. This is a FourCC with 0 as a special value meaning stream data.
        /// </summary>
        public uint Cmd => this.Ptr->Cmd;

        /// <summary>
        /// Allocated size in bytes of payload buffer.
        /// </summary>
        public uint AllocSize => this.Ptr->AllocSize;

        /// <summary>
        /// Number of bytes currently used in the payload buffer (starting from offset).
        /// </summary>
        public uint Length => this.Ptr->Length;

        /// <summary>
        /// Offset in bytes to the start of valid data in the payload buffer.
        /// </summary>
        public uint Offset => this.Ptr->Offset;

        /// <summary>
        /// Flags describing properties of a buffer header.
        /// </summary>
        public uint Flags => this.Ptr->Flags;

        /// <summary>
        /// Presentation timestamp in microseconds.
        /// </summary>
        public long Pts => this.Ptr->Pts;

        /// <summary>
        /// Decode timestamp in microseconds (dts = pts, except in the case of video streams with B frames).
        /// </summary>
        public long Dts => this.Ptr->Dts;

        /// <summary>
        /// Accessor to the specific type this buffer header represents.
        /// </summary>
        public MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type => Marshal.PtrToStructure<MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T>(this.Ptr->Type);

        /// <summary>
        /// List of properties associated with this buffer header.
        /// </summary>
        public List<MMALBufferProperties> Properties { get; }

        /// <summary>
        /// List of events associated with this buffer header.
        /// </summary>
        public List<int> Events { get; }

        /// <summary>
        /// Native pointer that represents this buffer header.
        /// </summary>
        public MMAL_BUFFER_HEADER_T* Ptr { get; }

        /// <summary>
        /// Creates a new Managed reference to a MMAL Buffer.
        /// </summary>
        /// <param name="ptr">The native pointer to the buffer.</param>
        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
            this.Properties = new List<MMALBufferProperties>();
            this.Events = new List<int>();
        }
        
        /// <summary>
        /// Print the properties associated with this buffer header to console.
        /// </summary>
        public void PrintProperties()
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug(this.ToString());
            }
        }

        /// <summary>
        /// Writes events associated with the buffer header to log.
        /// </summary>
        public void ParseEvents()
        {
            if (this.Cmd == MMALEvents.MMAL_EVENT_EOS)
            {
                MMALLog.Logger.LogDebug("Buffer event: MMAL_EVENT_EOS");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_ERROR)
            {
                MMALLog.Logger.LogDebug("Buffer event: MMAL_EVENT_ERROR");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_FORMAT_CHANGED)
            {
                MMALLog.Logger.LogDebug("Buffer event: MMAL_EVENT_FORMAT_CHANGED");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {
                MMALLog.Logger.LogDebug("Buffer event: MMAL_EVENT_PARAMETER_CHANGED");
            }
        }

        /// <summary>
        /// Checks whether a buffer header contains a certain status code.
        /// </summary>
        /// <param name="property">The status code.</param>
        /// <returns>True if the buffer header contains the status code.</returns>
        public bool AssertProperty(MMALBufferProperties property)
        {
            return ((int)this.Flags & (int)property) == (int)property;
        }

        /// <summary>
        /// Returns a summary, including length presentation timestamp and flags, about this buffer header instance.
        /// </summary>
        /// <returns>A string summarising this instance.</returns>
        public override string ToString()
        {
            this.InitialiseProperties();
            
            var sb = new StringBuilder();

            sb.Append(
                $"\r\n Buffer Header \r\n" +
                "---------------- \r\n" +
                $"Length: {this.Length} \r\n" +
                $"Presentation Timestamp: {this.Pts} \r\n" +
                $"Flags: \r\n");

            foreach (MMALBufferProperties prop in this.Properties)
            {
                sb.Append($"{prop.ToString()} \r\n");
            }

            sb.Append("---------------- \r\n");

            return sb.ToString();
        }

        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Gathers all data in this payload and returns as a byte array.
        /// </summary>
        /// <returns>A byte array containing the image frame.</returns>
        public byte[] GetBufferData()
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug("Getting data from buffer");
            }

            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");

            try
            {
                var ps = this.Ptr->data + this.Offset;
                var buffer = new byte[(int)this.Ptr->Length];
                Marshal.Copy((IntPtr)ps, buffer, 0, buffer.Length);
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                
                return buffer;
            }
            catch
            {
                // If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                MMALLog.Logger.LogWarning("Unable to handle data. Returning null.");
                return null;
            }
        }

        /// <summary>
        /// Writes user provided image data into a buffer header.
        /// </summary>
        /// <param name="source">The array of image data to write to buffer header.</param>
        /// <param name="length">The length of the data being written.</param>
        /// <param name="eof">Signal that we've reached the end of the input file.</param>
        public void ReadIntoBuffer(byte[] source, int length, bool eof)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"Reading {length} bytes into buffer");
            }
            
            this.Ptr->length = (uint)length;                        
            this.Ptr->dts = this.Ptr->pts = MMALUtil.MMAL_TIME_UNKNOWN;                        
            this.Ptr->offset = 0;

            if (eof)
            {
                this.Ptr->flags = (uint)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;
            }

            Marshal.Copy(source, 0, (IntPtr)this.Ptr->data, length);
        }

        /// <summary>
        /// Acquire a buffer header. Acquiring a buffer header increases a reference counter on it and makes 
        /// sure that the buffer header won't be recycled until all the references to it are gone.
        /// </summary>
        public void Acquire()
        {
            if (this.CheckState())
            {
                MMALBuffer.mmal_buffer_header_acquire(this.Ptr);
            }
        }

        /// <summary>
        /// Release a buffer header. Releasing a buffer header will decrease its reference counter and when no more references are left, 
        /// the buffer header will be recycled by calling its 'release' callback function.
        /// </summary>
        public void Release()
        {
            if (this.CheckState())
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.LogDebug("Releasing buffer.");
                }
                
                MMALBuffer.mmal_buffer_header_release(this.Ptr);
            }
            else
            {
                MMALLog.Logger.LogWarning("Buffer null, could not release.");
            }

            this.Dispose();
        }

        /// <summary>
        /// Reset a buffer header. Resets all header variables to default values.
        /// </summary>
        public void Reset()
        {
            if (this.CheckState())
            {
                MMALBuffer.mmal_buffer_header_reset(this.Ptr);
            }
        }

        /// <summary>
        /// Adds all properties associated with this buffer header to <see cref="Properties"/>.
        /// </summary>
        private void InitialiseProperties()
        {
            this.Properties.Clear();

            if (!this.CheckState())
            {
                return;
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END);
            }

            if (((int)this.Flags & ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START | (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)) == ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END))
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_COMPLETEFRAME);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
            }

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_NAL) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_NAL)
            {
                this.Properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_NAL);
            }
        }
    }
}
