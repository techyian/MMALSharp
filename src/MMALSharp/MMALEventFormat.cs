using MMALSharp.Native;
using System;
using System.Runtime.InteropServices;

namespace MMALSharp
{
    /// <summary>
    /// Represents an MMAL Event Format.
    /// </summary>
    public unsafe class MMALEventFormat
    {
        /// <summary>
        /// The FourCC code of the component.
        /// </summary>
        public string FourCC => MMALEncodingHelpers.ParseEncoding(this.Format.Encoding).EncodingName;
        
        /// <summary>
        /// The working bitrate of the component.
        /// </summary>
        public int Bitrate => this.Format.Bitrate;
        
        /// <summary>
        /// The working width of the component.
        /// </summary>
        public int Width => this.Format.Es->Video.Width;
        
        /// <summary>
        /// The working height of the component.
        /// </summary>
        public int Height => this.Format.Es->Video.Height;
        
        /// <summary>
        /// The region of interest X value of the component.
        /// </summary>
        public int CropX => this.Format.Es->Video.Crop.X;
        
        /// <summary>
        /// The region of interest Y value of the component.
        /// </summary>
        public int CropY => this.Format.Es->Video.Crop.Y;
        
        /// <summary>
        /// The region of interest width value of the component.
        /// </summary>
        public int CropWidth => this.Format.Es->Video.Crop.Width;
        
        /// <summary>
        /// The region of interest height value of the component.
        /// </summary>
        public int CropHeight => this.Format.Es->Video.Crop.Height;
        
        /// <summary>
        /// The pixel aspect ratio numerator value component.
        /// </summary>
        public int ParNum => this.Format.Es->Video.Par.Num;
        
        /// <summary>
        /// The pixel aspect ratio denominator value component.
        /// </summary>
        public int ParDen => this.Format.Es->Video.Par.Den;
        
        /// <summary>
        /// The framerate numerator value component.
        /// </summary>
        public int FramerateNum => this.Format.Es->Video.FrameRate.Num;
        
        /// <summary>
        /// The framerate denominator value component.
        /// </summary>
        public int FramerateDen => this.Format.Es->Video.FrameRate.Den;

        // Dereferenced struct.
        internal MMAL_ES_FORMAT_T Format { get; set; }

        /// <summary>
        /// Native pointer that represents this event format.
        /// </summary>
        internal MMAL_ES_FORMAT_T* Ptr { get; set; }

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

        internal static MMALEventFormat GetEventFormat(MMALBufferImpl buffer)
        {
            var ev = MMALEvents.mmal_event_format_changed_get(buffer.Ptr);                                    
            return new MMALEventFormat(Marshal.PtrToStructure<MMAL_ES_FORMAT_T>((IntPtr)ev->Format), ev->Format);
        }
    }
}
