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
    public unsafe class MMALBufferImpl : MMALObject
    {
        internal MMAL_BUFFER_HEADER_T* Ptr { get; set; }

        #region Buffer struct wrapper properties

        public byte* Data
        {
            get
            {
                return this.Ptr->data;
            }
        }

        public uint Cmd
        {
            get
            {
                return this.Ptr->cmd;
            }
        }

        public uint AllocSize {
            get
            {
                return this.Ptr->allocSize;
            }
        }
        public uint Length
        {
            get
            {
                return this.Ptr->length;
            }
        }
        public uint Offset
        {
            get
            {
                return this.Ptr->offset;
            }
        }
        public uint Flags
        {
            get
            {
                return this.Ptr->flags;
            }
        }
        public long Pts
        {
            get
            {
                return this.Ptr->pts;
            }
        }
        public long Dts
        {
            get
            {
                return this.Ptr->dts;
            }
        }
        public MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type
        {
            get
            {
                var t = Marshal.PtrToStructure<MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T>(this.Ptr->type);                                
                return t;
            }
        }

        #endregion

        public List<MMALBufferProperties> Properties { get; set; }

        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
            this.InitialiseProperties();
        }
              
        public void PrintProperties()
        {
            Console.WriteLine("---Begin buffer properties---");
            foreach(MMALBufferProperties prop in this.Properties)
            {
                Console.WriteLine(prop.ToString());
            }
            Console.WriteLine("---End buffer properties---");
        }
          
        public void InitialiseProperties()
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

        public byte[] GetBufferData()
        {
            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");
                        
            try
            {                
                var target = new byte[this.Ptr->length];
                                
                fixed(byte* pTarget = target)
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
            catch(Exception e)
            {
                //If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                if (MMALCameraConfigImpl.Config.Debug)
                    Console.WriteLine("Unable to handle data. Returning null.");
                return null;
            }            
        }

        public void Acquire()
        {
            MMALBuffer.mmal_buffer_header_acquire(this.Ptr);
        }

        public void Release()
        {
            MMALBuffer.mmal_buffer_header_release(this.Ptr);
        }

        public void Reset()
        {
            MMALBuffer.mmal_buffer_header_reset(this.Ptr);
        }
                
    }
}
