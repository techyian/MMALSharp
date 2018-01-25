using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a buffer header object
    /// </summary>
    public unsafe class MMALBufferImpl : MMALObject
    {
        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
            this.InitialiseProperties();
        }

        /// <summary>
        /// Native pointer that represents this buffer header
        /// </summary>
        internal MMAL_BUFFER_HEADER_T* Ptr { get; set; }

        /// <summary>
        /// Pointer to the data associated with this buffer header
        /// </summary>
        public byte* Data => this.Ptr->data;

        /// <summary>
        /// Defines what the buffer header contains. This is a FourCC with 0 as a special value meaning stream data
        /// </summary>
        public uint Cmd => this.Ptr->Cmd;

        /// <summary>
        /// Allocated size in bytes of payload buffer
        /// </summary>
        public uint AllocSize => this.Ptr->AllocSize;

        /// <summary>
        /// Number of bytes currently used in the payload buffer (starting from offset)
        /// </summary>
        public uint Length => this.Ptr->Length;

        /// <summary>
        /// Offset in bytes to the start of valid data in the payload buffer
        /// </summary>
        public uint Offset => this.Ptr->Offset;

        /// <summary>
        /// Flags describing properties of a buffer header
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
        /// Accessor to the specific type this buffer header represents
        /// </summary>
        public MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type => Marshal.PtrToStructure<MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T>(this.Ptr->Type);

        /// <summary>
        /// List of properties associated with this buffer header.
        /// </summary>
        public List<MMALBufferProperties> Properties { get; set; }

        /// <summary>
        /// List of events associated with this buffer header
        /// </summary>
        public List<int> Events { get; set; }

        /// <summary>
        /// Print the properties associated with this buffer header to console
        /// </summary>
        public void PrintProperties()
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug(this.ToString());
            }
        }

        public void ParseEvents()
        {
            if (this.Cmd == MMALEvents.MMAL_EVENT_EOS)
            {
                MMALLog.Logger.Debug("Buffer event: MMAL_EVENT_EOS");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_ERROR)
            {
                MMALLog.Logger.Debug("Buffer event: MMAL_EVENT_ERROR");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_FORMAT_CHANGED)
            {
                MMALLog.Logger.Debug("Buffer event: MMAL_EVENT_FORMAT_CHANGED");
            }

            if (this.Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
            {
                MMALLog.Logger.Debug("Buffer event: MMAL_EVENT_PARAMETER_CHANGED");
            }
        }

        public override string ToString()
        {
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

        /// <summary>
        /// Adds all properties associated with this buffer header to 'this.Properties'
        /// </summary>
        private void InitialiseProperties()
        {
            this.Properties = new List<MMALBufferProperties>();

            if (this.Ptr == null || (IntPtr)this.Ptr == IntPtr.Zero)
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
        }

        /// <summary>
        /// Gathers all data in this payload and returns as a byte array
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBufferData()
        {
            MMALLog.Logger.Debug("Getting data from buffer");

            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");

            try
            {
                var ps = this.Ptr->data + this.Offset;
                var buffer = Array.CreateInstance(typeof(byte), (int)this.Ptr->Length) as byte[];
                Marshal.Copy((IntPtr)ps, buffer, 0, buffer.Length);
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);

                return buffer;
            }
            catch
            {
                // If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                MMALLog.Logger.Warn("Unable to handle data. Returning null.");
                return null;
            }
        }

        internal void ReadIntoBuffer(byte[] source, int length, bool eof)
        {
            MMALLog.Logger.Debug("Reading data into buffer");

            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");
            var ptrAlloc = Marshal.AllocHGlobal(source.Length);
            this.Ptr->data = (byte*)ptrAlloc;
            this.Ptr->allocSize = (uint)source.Length;
            this.Ptr->length = (uint)length;

            if (eof)
            {
                this.Ptr->flags += (uint)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;
            }

            try
            {
                fixed (byte* pSource = source)
                {
                    var ps = pSource;

                    byte* pt = (byte*)ptrAlloc;

                    for (int i = 0; i < source.Length; i++)
                    {
                        *pt = *ps;
                        pt++;
                        ps++;
                    }
                }

                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
            }
            catch
            {
                // If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                Marshal.FreeHGlobal(ptrAlloc);
                MMALLog.Logger.Warn("Unable to write data to buffer.");
            }
        }

        /// <summary>
        /// Acquire a buffer header. Acquiring a buffer header increases a reference counter on it and makes 
        /// sure that the buffer header won't be recycled until all the references to it are gone.
        /// </summary>
        internal void Acquire()
        {
            if (this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero)
            {
                MMALBuffer.mmal_buffer_header_acquire(this.Ptr);
            }
        }

        /// <summary>
        /// Release a buffer header. Releasing a buffer header will decrease its reference counter and when no more references are left, 
        /// the buffer header will be recycled by calling its 'release' callback function.
        /// </summary>
        internal void Release()
        {
            if (this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero)
            {
                MMALBuffer.mmal_buffer_header_release(this.Ptr);
            }

            this.Dispose();
        }

        /// <summary>
        /// Reset a buffer header. Resets all header variables to default values.
        /// </summary>
        internal void Reset()
        {
            if (this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero)
            {
                MMALBuffer.mmal_buffer_header_reset(this.Ptr);
            }
        }        
    }
}
