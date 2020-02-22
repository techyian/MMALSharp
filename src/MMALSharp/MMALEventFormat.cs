using MMALSharp.Native;
using System;
using System.Runtime.InteropServices;
using MMALSharp.Common;

namespace MMALSharp
{
    /// <summary>
    /// Represents an MMAL Event Format.
    /// </summary>
    public unsafe class MMALEventFormat : IBufferEvent
    {
        /// <summary>
        /// Native pointer that represents this event format.
        /// </summary>
        public MMAL_ES_FORMAT_T* Ptr { get; }

        /// <summary>
        /// The FourCC code.
        /// </summary>
        public string FourCC => MMALEncodingHelpers.ParseEncoding(this.Format.Encoding).EncodingName;
        
        /// <summary>
        /// The working bitrate.
        /// </summary>
        public int Bitrate => this.Format.Bitrate;
        
        /// <summary>
        /// The width value.
        /// </summary>
        public int Width => this.Format.Es->Video.Width;
        
        /// <summary>
        /// The height value.
        /// </summary>
        public int Height => this.Format.Es->Video.Height;
        
        /// <summary>
        /// The CropX value.
        /// </summary>
        public int CropX => this.Format.Es->Video.Crop.X;
        
        /// <summary>
        /// The CropY value.
        /// </summary>
        public int CropY => this.Format.Es->Video.Crop.Y;
        
        /// <summary>
        /// The crop width value.
        /// </summary>
        public int CropWidth => this.Format.Es->Video.Crop.Width;
        
        /// <summary>
        /// The crop height value.
        /// </summary>
        public int CropHeight => this.Format.Es->Video.Crop.Height;
        
        /// <summary>
        /// The pixel aspect ratio numerator value.
        /// </summary>
        public int ParNum => this.Format.Es->Video.Par.Num;

        /// <summary>
        /// The pixel aspect ratio denominator value.
        /// </summary>
        public int ParDen => this.Format.Es->Video.Par.Den;
        
        /// <summary>
        /// The framerate numerator value.
        /// </summary>
        public int FramerateNum => this.Format.Es->Video.FrameRate.Num;
        
        /// <summary>
        /// The framerate denominator value.
        /// </summary>
        public int FramerateDen => this.Format.Es->Video.FrameRate.Den;

        // De-referenced struct.
        private MMAL_ES_FORMAT_T Format { get; }

        /// <summary>
        /// Creates a new instance of <see cref="MMALEventFormat"/>.
        /// </summary>
        /// <param name="format">The native struct.</param>
        public MMALEventFormat(MMAL_ES_FORMAT_T format)
        {
            this.Format = format;
        }

        /// <summary>
        /// Creates a new instance of <see cref="MMALEventFormat"/>.
        /// </summary>
        /// <param name="format">The native struct.</param>
        /// <param name="ptr">The native pointer.</param>
        public MMALEventFormat(MMAL_ES_FORMAT_T format, MMAL_ES_FORMAT_T* ptr)
        {
            this.Format = format;
            this.Ptr = ptr;
        }

        internal static MMALEventFormat GetEventFormat(IBuffer buffer)
        {
            var ev = MMALEvents.mmal_event_format_changed_get(buffer.Ptr);                                    
            return new MMALEventFormat(Marshal.PtrToStructure<MMAL_ES_FORMAT_T>((IntPtr)ev->Format), ev->Format);
        }
    }
}
