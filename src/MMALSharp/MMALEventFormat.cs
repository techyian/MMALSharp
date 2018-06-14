using MMALSharp.Native;
using System;
using System.Runtime.InteropServices;

namespace MMALSharp
{
    public unsafe class MMALEventFormat
    {
        public string FourCC => MMALEncodingHelpers.ParseEncoding(this.Format.Encoding).EncodingName;
        public int Bitrate => this.Format.Bitrate;
        public int Width => this.Format.Es->Video.Width;
        public int Height => this.Format.Es->Video.Height;
        public int CropX => this.Format.Es->Video.Crop.X;
        public int CropY => this.Format.Es->Video.Crop.Y;
        public int CropWidth => this.Format.Es->Video.Crop.Width;
        public int CropHeight => this.Format.Es->Video.Crop.Height;
        public int ParNum => this.Format.Es->Video.Par.Num;
        public int ParDen => this.Format.Es->Video.Par.Den;
        public int FramerateNum => this.Format.Es->Video.Framerate.Num;
        public int FramerateDen => this.Format.Es->Video.Framerate.Den;

        // Dereferenced struct.
        internal MMAL_ES_FORMAT_T Format { get; set; }

        /// <summary>
        /// Native pointer that represents this event format.
        /// </summary>
        internal MMAL_ES_FORMAT_T* Ptr { get; set; }

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
