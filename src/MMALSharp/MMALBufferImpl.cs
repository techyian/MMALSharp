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
        /// <summary>
        /// Native pointer that represents this buffer header
        /// </summary>
        internal MMAL_BUFFER_HEADER_T* Ptr { get; set; }

        #region Buffer struct wrapper properties

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
       
        #endregion

        /// <summary>
        /// List of properties associated with this buffer header.
        /// </summary>
        public List<MMALBufferProperties> Properties { get; set; }

        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
            this.InitialiseProperties();
        }
              
        /// <summary>
        /// Print the properties associated with this buffer header to console
        /// </summary>
        public void PrintProperties()
        {
            if(MMALCameraConfig.Debug)
                MMALLog.Logger.Debug(this.ToString());            
        }

        /// <summary>
        /// Adds all properties associated with this buffer header to 'this.Properties'
        /// </summary>
        private void InitialiseProperties()
        {
            List<MMALBufferProperties> properties = new List<MMALBufferProperties>();

            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END);
            }
            if (((int)this.Flags & ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START | (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)) == ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END))
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_COMPLETEFRAME);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED);
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                properties.Add(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
            }

            this.Properties = properties;             
        }

        /// <summary>
        /// Gathers all data in this payload and returns as a byte array
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBufferData()
        {
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
                //If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                MMALLog.Logger.Warn("Unable to handle data. Returning null.");
                return null;
            }            
        }

        internal void ReadIntoBuffer(byte[] source, bool eof)
        {
            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");
            var ptrAlloc = Marshal.AllocHGlobal(source.Length);
            this.Ptr->data = (byte*)ptrAlloc;
            this.Ptr->allocSize = (uint)source.Length;
            this.Ptr->length = (uint)source.Length;

            if (eof)
            {
                this.Ptr->flags = (uint)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;
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
                //If something goes wrong, unlock the header.
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
            MMALBuffer.mmal_buffer_header_acquire(this.Ptr);
        }

        /// <summary>
        /// Release a buffer header. Releasing a buffer header will decrease its reference counter and when no more references are left, 
        /// the buffer header will be recycled by calling its 'release' callback function.
        /// </summary>
        internal void Release()
        {
            MMALBuffer.mmal_buffer_header_release(this.Ptr);
            this.Dispose();
        }

        /// <summary>
        /// Reset a buffer header. Resets all header variables to default values.
        /// </summary>
        internal void Reset()
        {
            MMALBuffer.mmal_buffer_header_reset(this.Ptr);
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
    }
}
