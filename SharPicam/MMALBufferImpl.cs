using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

namespace SharPicam
{
    public unsafe class MMALBufferImpl : MMALObject
    {
        public MMAL_BUFFER_HEADER_T* Ptr { get; set; }

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
            
        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
        }
                
        public MMALBufferProperties Properties()
        {
            Console.WriteLine("-- Buffer Properties --");
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS)
            {
                Console.WriteLine("EOS");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START)
            {
                Console.WriteLine("Frame start");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)
            {
                Console.WriteLine("Frame end");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END;
            }
            if (((int)this.Flags & ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START | (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END)) == ((int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_START & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END))
            {
                Console.WriteLine("Complete frame");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_COMPLETEFRAME;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME)
            {
                Console.WriteLine("Key frame");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_KEYFRAME;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY)
            {
                Console.WriteLine("Discontinuity");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG)
            {
                Console.WriteLine("Config");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED)
            {
                Console.WriteLine("Encrypted");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO)
            {
                Console.WriteLine("Codec Side Info");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT)
            {
                Console.WriteLine("Snapshot");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED)
            {
                Console.WriteLine("Corrupted");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CORRUPTED;
            }
            if (((int)this.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                Console.WriteLine("Transmission failed");
                return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED;
            }

            Console.WriteLine("-- End buffer properties --");
            return MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_UNKNOWN;            
        }

        public Tuple<MMALBufferProperties, UnmanagedMemoryStream> DataStream()
        {
            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");

            //When disposing of the memory stream, it is vital that the call to MMALBuffer.mmal_buffer_header_mem_unlock is called.
            try
            {
                var ptr = this.Ptr->data + this.Offset;
                                
                var stream = new UnmanagedMemoryStream(ptr, this.Length, this.Length, FileAccess.ReadWrite);
                return new Tuple<MMALBufferProperties, UnmanagedMemoryStream>(this.Properties(), stream);
            }
            catch(Exception e)
            {
                //If something goes wrong, unlock the header.
                MMALBuffer.mmal_buffer_header_mem_unlock(this.Ptr);
                Console.WriteLine("Unable to handle data stream. Returning null.");
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
