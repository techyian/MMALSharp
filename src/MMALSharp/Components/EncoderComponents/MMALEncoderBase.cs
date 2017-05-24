using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Handlers;
using MMALSharp.Components.EncoderComponents;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components
    /// </summary>
    public abstract class MMALEncoderBase : MMALDownstreamComponent, IConvert
    {   
        public MMALPortImpl InputPort { get; set; }
        public MMALPortImpl OutputPort { get; set; }

        protected MMALEncoderBase(string encoderName, ICaptureHandler handler) : base(encoderName, handler)
        {
            this.Inputs.ElementAt(0).ShallowCopy(this.Outputs.ElementAt(0));
        }

        /// <summary>
        /// Call to configure changes on an Image Encoder input port. Used when providing an image file directly
        /// to the component.
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="framerate"></param>
        /// <param name="bitrate"></param>
        /// <param name="headerByteSize"></param>
        /// <param name="flags">Bitwise value of flags describing the difference between source and target elementary streams. See class MMALFormat and the url http://www.jvcref.com/files/PI/documentation/html/group___mmal_format.html#ga26178f5c56486c3e5db7f5f7c90598a0 </param>
        public virtual unsafe void ConfigureInputPort(MMALEncoding encodingType, int width, int height, MMAL_RATIONAL_T framerate, 
                                                      int bitrate, int headerByteSize, int flags)
        {
            this.InputPort.Ptr->Format->type = MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_VIDEO;
            this.InputPort.Ptr->Format->encoding = encodingType.EncodingVal;
            this.InputPort.Ptr->Format->es->video.height = height;
            this.InputPort.Ptr->Format->es->video.width = width;
            this.InputPort.Ptr->Format->flags = flags;
            this.InputPort.Ptr->Format->bitrate = bitrate;

            MMALFormat.mmal_format_extradata_alloc(this.InputPort.Ptr->Format, (uint)headerByteSize);

            this.InputPort.Ptr->Format->extraDataSize = headerByteSize;

            //Allocate the correct size for extraData. This will be cleared by MMAL upon disposal.
            IntPtr extraDataAlloc = Marshal.AllocHGlobal(headerByteSize);
            this.InputPort.Ptr->Format->extraData = extraDataAlloc;

            try
            {
                this.InputPort.Commit();
            }
            catch
            {
                //If fail, cleanup unmanaged memory allocation.
                Marshal.FreeHGlobal(extraDataAlloc);
            }

            if (this.OutputPort.Ptr->Format->type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                Marshal.FreeHGlobal(extraDataAlloc);
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            this.InputPort.Ptr->BufferNum = Math.Max(this.InputPort.Ptr->BufferNumRecommended, this.InputPort.Ptr->BufferNumMin);
            this.InputPort.Ptr->BufferSize = Math.Max(this.InputPort.Ptr->BufferSizeRecommended, this.InputPort.Ptr->BufferSizeMin);

            this.InputPort.BufferPool = new MMALPoolImpl(this.InputPort);

            this.InputPort.EncodingType = encodingType;
            this.InputPort.Width = width;
            this.InputPort.Height = height;

        }

        /// <summary>
        /// Call to configure changes on an Image Encoder output port.
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="pixelFormat"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="framerate"></param>
        /// <param name="bitrate"></param>
        /// <param name="quality"></param>
        public virtual unsafe void ConfigureOutputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height,
                                                       MMAL_RATIONAL_T framerate, int bitrate, int quality = 90)
        {
            this.OutputPort.Ptr->Format->encoding = encodingType.EncodingVal;
            this.OutputPort.Ptr->Format->encodingVariant = pixelFormat.EncodingVal;

            MMAL_VIDEO_FORMAT_T tempVid = this.OutputPort.Ptr->Format->es->video;

            MMAL_VIDEO_FORMAT_T newVid = new MMAL_VIDEO_FORMAT_T(
                MMALUtil.VCOS_ALIGN_UP(width, 32),
                MMALUtil.VCOS_ALIGN_UP(height, 16),
                new MMAL_RECT_T(0, 0, width, height),
                framerate,
                tempVid.par,
                tempVid.colorSpace
            );

            this.OutputPort.Ptr->Format->es->video = newVid;
            this.OutputPort.Ptr->Format->bitrate = bitrate;

            try
            {
                this.OutputPort.Commit();
            }
            catch
            {
                //If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                Helpers.PrintWarning("Commit of output port failed. Attempting to reset values.");
                this.OutputPort.Ptr->Format->es->video = tempVid;
                this.OutputPort.Commit();
            }

            if (encodingType == MMALEncoding.MMAL_ENCODING_JPEG)
            {
                this.OutputPort.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, quality);
            }

            this.OutputPort.BufferPool = new MMALPoolImpl(this.OutputPort);

            this.OutputPort.EncodingType = encodingType;
            this.OutputPort.PixelFormat = pixelFormat;

            this.OutputPort.Ptr->BufferNum = Math.Max(this.OutputPort.Ptr->BufferNumRecommended, this.OutputPort.Ptr->BufferNumMin);
            this.OutputPort.Ptr->BufferSize = Math.Max(this.OutputPort.Ptr->BufferSizeRecommended, this.OutputPort.Ptr->BufferSizeMin);

            this.OutputPort.Width = width;
            this.OutputPort.Height = height;

        }

        /// <summary>
        /// Annotates the image with various text as specified in the MMALCameraConfig class.
        /// </summary>
        internal unsafe void AnnotateImage()
        {
            if (MMALCameraConfig.Annotate != null)
            {
                if (MMALCameraConfig.Debug)
                    Console.WriteLine("Setting annotate");
                                                               
                var sb = new StringBuilder();

                var showShutter = 0;
                var showAnalogGain = 0;
                var showLens = 0;
                var showCaf = 0;
                var showMotion = 0;
                var showFrame = 0;
                var enableTextBackground = 0;
                var textSize = (byte)0;
                var customTextColor = 0;
                var customTextY = (byte)0;
                var customTextU = (byte)0;
                var customTextV = (byte)0;
                var customBackgroundColor = 0;
                var customBackgroundY = (byte)0;
                var customBackgroundU = (byte)0;
                var customBackgroundV = (byte)0;
                
                if (!string.IsNullOrEmpty(MMALCameraConfig.Annotate.CustomText))
                    sb.Append(MMALCameraConfig.Annotate.CustomText + " ");

                if (MMALCameraConfig.Annotate.ShowTimeText)
                    sb.Append(DateTime.Now.ToString("HH:mm") + " ");

                if (MMALCameraConfig.Annotate.ShowDateText)
                    sb.Append(DateTime.Now.ToString("dd/MM/yyyy") + " ");

                if (MMALCameraConfig.Annotate.ShowShutterSettings)
                    showShutter = 1;

                if (MMALCameraConfig.Annotate.ShowGainSettings)
                    showAnalogGain = 1;

                if (MMALCameraConfig.Annotate.ShowLensSettings)
                    showLens = 1;

                if (MMALCameraConfig.Annotate.ShowCafSettings)
                    showCaf = 1;

                if (MMALCameraConfig.Annotate.ShowMotionSettings)
                    showMotion = 1;

                if (MMALCameraConfig.Annotate.ShowFrameNumber)
                    showFrame = 1;

                if (MMALCameraConfig.Annotate.ShowBlackBackground)
                    enableTextBackground = 1;

                textSize = System.Convert.ToByte(MMALCameraConfig.Annotate.TextSize);

                if (MMALCameraConfig.Annotate.TextColour != -1)
                {
                    customTextColor = 1;
                    customTextY = System.Convert.ToByte((MMALCameraConfig.Annotate.TextColour & 0xff));
                    customTextU = System.Convert.ToByte(((MMALCameraConfig.Annotate.TextColour >> 8) & 0xff));
                    customTextV = System.Convert.ToByte(((MMALCameraConfig.Annotate.TextColour >> 16) & 0xff));
                }
                
                if (MMALCameraConfig.Annotate.BgColour != -1)
                {
                    customBackgroundColor = 1;
                    customBackgroundY = System.Convert.ToByte((MMALCameraConfig.Annotate.BgColour & 0xff));
                    customBackgroundU = System.Convert.ToByte(((MMALCameraConfig.Annotate.BgColour >> 8) & 0xff));
                    customBackgroundV = System.Convert.ToByte(((MMALCameraConfig.Annotate.BgColour >> 16) & 0xff));
                }
                
                string t = sb.ToString() + char.MinValue;
                                
                var text = Encoding.ASCII.GetBytes(t);
                                
                var str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, (Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (t.Length))),
                                                                                                    1, showShutter, showAnalogGain, showLens,
                                                                                                    showCaf, showMotion, showFrame, enableTextBackground,
                                                                                                    customBackgroundColor, customBackgroundY, customBackgroundU, customBackgroundV, (byte)0, customTextColor,
                                                                                                    customTextY, customTextU, customTextV, textSize, text);
                
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
                Marshal.StructureToPtr(str, ptr, false);

                MMALCheck(MMALPort.mmal_port_parameter_set(MMALCamera.Instance.Camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate");

                Marshal.FreeHGlobal(ptr);
            }
        }
        
        public override void Dispose()
        {                        
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Removing encoder");
            }
            
            this.Connection?.Destroy();
            
            MMALCamera.Instance.Encoders.Remove(this);

            //Remove any unmanaged resources held by the capture handler.
            this.Handler?.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Encodes/decodes user provided image data
        /// </summary>
        /// <param name="handler">The capture handler containing user provided data</param>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        /// <returns>An awaitable task</returns>
        public virtual async Task Convert(int outputPort = 0)
        {
            //Enable both input and output ports. Ports should have been pre-configured by user prior to this point.
            this.Start(this.InputPort, this.ManagedInputCallback);
            this.Start(this.OutputPort, this.ManagedOutputCallback);

            this.EnableComponent();

            //Wait until the process is complete.
            this.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }
    }
}
