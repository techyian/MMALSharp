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
        /// Enables processing on the encoder's output port
        /// </summary>
        public void Start()
        {
            this.Outputs.ElementAt(0).EnablePort(this.EncoderOutputCallback);
        }

        /// <summary>
        /// Stops processing on the encoder's output port
        /// </summary>
        public void Stop()
        {
            this.Outputs.ElementAt(0).DisablePort();
        }

        public virtual void EncoderInputCallback(MMALBufferImpl buffer)
        {
            buffer.Release();
        }

        /// <summary>
        /// Delegate to process the buffer header containing image data
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void EncoderOutputCallback(MMALBufferImpl buffer)
        {
            var data = buffer.GetBufferData();

            if (data != null && this.Storage != null)
                this.Storage = this.Storage.Concat(data).ToArray();
            else if (data != null && this.Storage == null)
                this.Storage = data;
        }

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
        public MMALVideoEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.Initialize();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
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
        public uint EncodingType { get; set; }

        /// <summary>
        /// The quality of the JPEG image
        /// </summary>
        public uint Quality { get; set; }

        public MMALImageEncoder(uint encodingType, uint quality) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            this.EncodingType = encodingType;
            this.Quality = quality;
            this.Initialize();
        }

        public MMALImageEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            this.EncodingType = MMALEncodings.MMAL_ENCODING_JPEG;
            this.Quality = 90;
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
        public unsafe void AddExifTag(ExifTag exifTag)
        {
            this.SetDisableExif(false);            
            var formattedExif = exifTag.Key + "=" + exifTag.Value + char.MinValue;
          
            if (formattedExif.Length > MaxExifPayloadLength)
                throw new PiCameraError("EXIF payload greater than allowed max.");
            
            var bytes = Encoding.ASCII.GetBytes(formattedExif);
            
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T>() + (bytes.Length - 1));

            var str = new MMAL_PARAMETER_EXIF_T(new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCamera.MMAL_PARAMETER_EXIF,
                                                (uint)(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T_DUMMY>() + (bytes.Length - 1))
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
