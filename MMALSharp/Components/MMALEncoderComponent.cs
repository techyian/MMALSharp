using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALParameterHelpers;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components
    /// </summary>
    public abstract unsafe class MMALEncoderBase : MMALDownstreamComponent
    {        
        protected MMALEncoderBase(string encoderName) : base(encoderName) { }
        
        /// <summary>
        /// Initializes the encoder component to allow processing to commence. Creates the same format between input/output port.
        /// </summary>
        public override void Initialize()
        {
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);

            input.ShallowCopy(output);
        }

    }
        
    /// <summary>
    /// Represents a video encoder component
    /// </summary>
    public unsafe class MMALVideoEncoder : MMALEncoderBase
    {
        /// <summary>
        /// The encoding type that the video encoder should use
        /// </summary>
        public int EncodingType { get; set; } = MMALEncodings.MMAL_ENCODING_H264;

        public int Bitrate { get; set; } = 17000000;

        public int Framerate { get; set; } = 30;

        public MMALVideoEncoder(int encodingType, int bitrate, int framerate) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.EncodingType = encodingType;
            this.Bitrate = bitrate;
        }

        public MMALVideoEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {            
        }

        public override void ManagedCallback(MMALBufferImpl buffer, MMALPortBase port)
        {


            base.ManagedCallback(buffer, port);
        }

    }

    /// <summary>
    /// Represents a video decoder component
    /// </summary>
    public unsafe class MMALVideoDecoder : MMALEncoderBase
    {
        public MMALVideoDecoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER)
        {
            this.Initialize();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
        
    }

    /// <summary>
    /// Represents an image encoder component
    /// </summary>
    public unsafe class MMALImageEncoder : MMALEncoderBase
    {
        public const int MaxExifPayloadLength = 128;

        /// <summary>
        /// The encoding type that the image encoder should use
        /// </summary>
        public int EncodingType { get; set; } = MMALEncodings.MMAL_ENCODING_JPEG;

        /// <summary>
        /// The quality of the JPEG image
        /// </summary>
        public int Quality { get; set; } = 90;

        public MMALImageEncoder(int encodingType, int quality) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            if(encodingType > 0)
                this.EncodingType = encodingType;
            if(quality > 0)
                this.Quality = quality;
            this.Initialize();
        }

        public MMALImageEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {           
            this.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);
                        
            output.Ptr->format->encoding = this.EncodingType;
            output.Ptr->bufferNum = Math.Max(output.Ptr->bufferNumRecommended, output.Ptr->bufferNumMin);
            output.Ptr->bufferSize = Math.Max(output.Ptr->bufferSizeRecommended, output.Ptr->bufferSizeMin);

            output.Commit();
                        
            if (this.EncodingType == MMALEncodings.MMAL_ENCODING_JPEG)
                SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, this.Quality, output.Ptr);
        }

        /// <summary>
        /// Provides a facility to add an EXIF tag to the image. 
        /// </summary>
        /// <param name="exifTag"></param>
        internal unsafe void AddExifTag(ExifTag exifTag)
        {
            this.SetDisableExif(false);            
            var formattedExif = exifTag.Key + "=" + exifTag.Value + char.MinValue;
          
            if (formattedExif.Length > MaxExifPayloadLength)
                throw new PiCameraError("EXIF payload greater than allowed max.");
            
            var bytes = Encoding.ASCII.GetBytes(formattedExif);
            
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T>() + (bytes.Length - 1));

            var str = new MMAL_PARAMETER_EXIF_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_EXIF,
                                                (Marshal.SizeOf<MMAL_PARAMETER_EXIF_T_DUMMY>() + (bytes.Length - 1))
                                                                            ), 0, 0, 0, bytes);

            Marshal.StructureToPtr(str, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs.ElementAt(0).Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), string.Format("Unable to set EXIF {0}", formattedExif));

            Marshal.FreeHGlobal(ptr);
        }

    }

    /// <summary>
    /// Represents an image decoder component
    /// </summary>
    public unsafe class MMALImageDecoder : MMALEncoderBase
    {
        public MMALImageDecoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER)
        {
            this.Initialize();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

    }


}
