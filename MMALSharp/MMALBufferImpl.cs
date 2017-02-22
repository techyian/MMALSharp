using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        public byte* Data
        {
            get
            {
                return this.Ptr->data;
            }
        }

        /// <summary>
        /// Defines what the buffer header contains. This is a FourCC with 0 as a special value meaning stream data
        /// </summary>
        public uint Cmd
        {
            get
            {
                return this.Ptr->cmd;
            }
        }

        /// <summary>
        /// Allocated size in bytes of payload buffer
        /// </summary>
        public uint AllocSize {
            get
            {
                return this.Ptr->allocSize;
            }
        }

        /// <summary>
        /// Number of bytes currently used in the payload buffer (starting from offset)
        /// </summary>
        public uint Length
        {
            get
            {
                return this.Ptr->length;
            }
        }

        /// <summary>
        /// Offset in bytes to the start of valid data in the payload buffer
        /// </summary>
        public uint Offset
        {
            get
            {
                return this.Ptr->offset;
            }
        }

        /// <summary>
        /// Flags describing properties of a buffer header
        /// </summary>
        public uint Flags
        {
            get
            {
                return this.Ptr->flags;
            }
        }

        /// <summary>
        /// Presentation timestamp in microseconds.
        /// </summary>
        public long Pts
        {
            get
            {
                return this.Ptr->pts;
            }
        }

        /// <summary>
        /// Decode timestamp in microseconds (dts = pts, except in the case of video streams with B frames).
        /// </summary>
        public long Dts
        {
            get
            {
                return this.Ptr->dts;
            }
        }

        /// <summary>
        /// Accessor to the specific type this buffer header represents
        /// </summary>
        public MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type
        {
            get
            {
                var t = Marshal.PtrToStructure<MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T>(this.Ptr->type);                                
                return t;
            }
        }

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
            Console.WriteLine("---Begin buffer properties---");
            foreach (MMALBufferProperties prop in this.Properties)
            {
                Console.WriteLine(prop.ToString());
            }
            Console.WriteLine("---End buffer properties---");
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
                var target = new byte[this.Ptr->length];
                                
                fixed (byte* pTarget = target)
                {
                    var pt = pTarget;
                    var ps = this.Ptr->data + this.Offset;
                    
                    for (int i = 0; i < this.Ptr->length; i++)
                    {
                        *pt = *ps;
                        pt++;
                        ps++;
                    }
                }

                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                return target;                                
            }
            catch
            {
                //If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                if (MMALCameraConfigImpl.Config.Debug)
                    Console.WriteLine("Unable to handle data. Returning null.");
                return null;
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
        }

        /// <summary>
        /// Reset a buffer header. Resets all header variables to default values.
        /// </summary>
        internal void Reset()
        {
            MMALBuffer.mmal_buffer_header_reset(this.Ptr);
        }
                
    }
}
