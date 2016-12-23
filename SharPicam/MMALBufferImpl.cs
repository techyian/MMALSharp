using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALBufferImpl : MMALObject
    {
        public MMAL_BUFFER_HEADER_T* Ptr { get; set; }
        public uint AllocSize {
            get
            {
                return (*this.Ptr).allocSize;
            }
        }
        public uint Length
        {
            get
            {
                return (*this.Ptr).length;
            }
        }
        public uint Offset
        {
            get
            {
                return (*this.Ptr).offset;
            }
        }
        public uint Flags
        {
            get
            {
                return (*this.Ptr).flags;
            }
        }
        public long Pts
        {
            get
            {
                return (*this.Ptr).pts;
            }
        }
        public long Dts
        {
            get
            {
                return (*this.Ptr).dts;
            }
        }

        
        public MMALBufferImpl(MMAL_BUFFER_HEADER_T* ptr)
        {
            this.Ptr = ptr;
        }


        public void Properties()
        {
            Console.WriteLine("-- Buffer Properties --");
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_EOS) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_EOS)
            {
                Console.WriteLine("EOS");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_START) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_START)
            {
                Console.WriteLine("Frame start");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_END) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_END)
            {
                Console.WriteLine("Frame end");
            }
            if (((int)this.Flags & (MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_START | MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_END)) == (MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_START & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_FRAME_END))
            {
                Console.WriteLine("Complete frame");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_KEYFRAME) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_KEYFRAME)
            {
                Console.WriteLine("Key frame");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_DISCONTINUITY)
            {
                Console.WriteLine("Discontinuity");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CONFIG) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CONFIG)
            {
                Console.WriteLine("Config");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_ENCRYPTED)
            {
                Console.WriteLine("Encrypted");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO)
            {
                Console.WriteLine("Codec Side Info");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT) == MMALBuffer.MMAL_BUFFER_HEADER_FLAGS_SNAPSHOT)
            {
                Console.WriteLine("Snapshot");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CORRUPTED) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_CORRUPTED)
            {
                Console.WriteLine("Corrupted");
            }
            if (((int)this.Flags & MMALBuffer.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED) == MMALBuffer.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                Console.WriteLine("Transmission failed");
            }
            Console.WriteLine("-- End buffer properties --");
        }

        public UnmanagedMemoryStream DataStream()
        {
            MMALCheck(MMALBuffer.mmal_buffer_header_mem_lock(this.Ptr), "Unable to lock buffer header.");

            //When disposing of the memory stream, it is vital that the call to MMALBuffer.mmal_buffer_header_mem_unlock is called.
            try
            {
                var stream = new UnmanagedMemoryStream((*this.Ptr).data, this.Length, this.Length, FileAccess.ReadWrite);
                return stream;
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
