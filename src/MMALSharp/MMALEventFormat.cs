using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public unsafe class MMALEventFormat
    {        
        // Dereferenced struct
        internal MMAL_ES_FORMAT_T Format { get; set; }

        /// <summary>
        /// Native pointer that represents this event format
        /// </summary>
        internal MMAL_ES_FORMAT_T* Ptr { get; set; }

        public string FourCC => MMALEncodingHelpers.ParseEncoding(this.Format.encoding).EncodingName;
        public int Bitrate => this.Format.bitrate;
        public int Width => this.Format.es->video.width;
        public int Height => this.Format.es->video.height;
        public int CropX => this.Format.es->video.crop.X;
        public int CropY => this.Format.es->video.crop.Y;
        public int CropWidth => this.Format.es->video.crop.Width;
        public int CropHeight => this.Format.es->video.crop.Height;
        public int ParNum => this.Format.es->video.par.Num;
        public int ParDen => this.Format.es->video.par.Den;
        public int FramerateNum => this.Format.es->video.frameRate.Num;
        public int FramerateDen => this.Format.es->video.frameRate.Den;
        
        public MMALEventFormat(MMAL_ES_FORMAT_T format)
        {
            this.Format = format;
        }

        public MMALEventFormat(MMAL_ES_FORMAT_T format, MMAL_ES_FORMAT_T* ptr)
        {
            this.Format = format;
            this.Ptr = ptr;
        }

        internal static MMALEventFormat GetEventFormat(MMALBufferImpl buffer)
        {
            var ev = MMALEvents.mmal_event_format_changed_get(buffer.Ptr);                                    
            return new MMALEventFormat(Marshal.PtrToStructure<MMAL_ES_FORMAT_T>((IntPtr)ev->Format), ev->Format);
        }
    }
}
