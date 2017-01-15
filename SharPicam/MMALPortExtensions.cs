using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;
using static SharPicam.MMALParameterHelpers;
using static SharPicam.Native.MMALParameters;
using static SharPicam.Native.MMALParametersCamera;

namespace SharPicam
{
    public unsafe static class MMALPortExtensions
    {
        public static void SetControlParameters(this MMALControlPortImpl port, MMALCameraParameters parameters)
        {
            SetSaturation(port, parameters.Saturation);
            SetSharpness(port, parameters.Sharpness);
            SetContrast(port, parameters.Contrast);
            SetBrightness(port, parameters.Brightness);
            SetISO(port, parameters.ISO);
            SetVideoStabilisation(port, parameters.VideoStabilisation);
            SetExposureCompensation(port, parameters.ExposureCompensation);
            SetExposureMode(port, parameters.ExposureMode);
            SetMeteringMode(port, parameters.ExposureMeterMode);
            SetAwbMode(port, parameters.AwbMode);
            SetAwbGains(port, parameters.AwbGainsR, parameters.AwbGainsB);
            SetImageFx(port, parameters.ImageEffect);
            SetColourFx(port, parameters.Effects);            
        }

        public static void SetCameraConfig(this MMALControlPortImpl port, MMAL_PARAMETER_CAMERA_CONFIG_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &value.hdr), "Unable to set camera config.");
        }

        public static void SetChangeEventRequest(this MMALControlPortImpl port, MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &value.hdr), "Unable to set camera event request.");
        }

        public static void SetSaturation(this MMALPortBase port, int saturation)
        {
            var value = new MMAL_RATIONAL_T(saturation, 100);

            if (saturation >= -100 && saturation <= 100)
            {
                SetParameter(MMAL_PARAMETER_SATURATION, value, port.Ptr);
            }
            else
            {
                throw new Exception("Invalid saturation value");
            }
                
        }

        public static void SetSharpness(this MMALPortBase port, int sharpness)
        {
            var value = new MMAL_RATIONAL_T(sharpness, 100);

            if (sharpness >= -100 && sharpness <= 100)
            {
                SetParameter(MMAL_PARAMETER_SHARPNESS, value, port.Ptr);
            }
            else
            {
                throw new Exception("Invalid sharpness value");
            }
                        
        }
        public static void SetContrast(this MMALPortBase port, int contrast)
        {
            var value = new MMAL_RATIONAL_T(contrast, 100);

            if (contrast >= -100 && contrast <= 100)
            {
                SetParameter(MMAL_PARAMETER_CONTRAST, value, port.Ptr);
            }
            else
            {
                throw new Exception("Invalid contrast value");
            }
                     
        }

        public static void SetBrightness(this MMALPortBase port, int brightness)
        {
            var value = new MMAL_RATIONAL_T(brightness, 100);

            if (brightness >= -100 && brightness <= 100)
            {
                SetParameter(MMAL_PARAMETER_BRIGHTNESS, value, port.Ptr);
            }
            else
            {
                throw new Exception("Invalid brightness value");
            }
            
        }

        public static void SetISO(this MMALPortBase port, int iso)
        {
            SetParameter(MMAL_PARAMETER_ISO, iso, port.Ptr);                        
        }

        public static void SetVideoStabilisation(this MMALPortBase port, bool vstabilisation)
        {
            SetParameter(MMAL_PARAMETER_VIDEO_STABILISATION, vstabilisation, port.Ptr);            
        }

        public static void SetExposureCompensation(this MMALPortBase port, int expCompensation)
        {
            SetParameter(MMAL_PARAMETER_EXPOSURE_COMP, expCompensation, port.Ptr);
        }

        public static void SetExposureMode(this MMALPortBase port, MMAL_PARAM_EXPOSUREMODE_T mode)
        {
            MMAL_PARAMETER_EXPOSUREMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMODE_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_EXPOSURE_MODE, (uint)Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &exp_mode.hdr), "Unable to set exposure mode");
        }

        public static void SetMeteringMode(this MMALPortBase port, MMAL_PARAM_EXPOSUREMETERINGMODE_T mode)
        {
            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_EXP_METERING_MODE, (uint)Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &exp_mode.hdr), "Unable to set exposure metering mode");
        }

        public static void SetAwbMode(this MMALPortBase port, MMAL_PARAM_AWBMODE_T mode)
        {
            MMAL_PARAMETER_AWBMODE_T awb_mode = new MMAL_PARAMETER_AWBMODE_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_AWB_MODE, (uint)Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &awb_mode.hdr), "Unable to set awb mode");
        }

        public static void SetAwbGains(this MMALPortBase port, double r_gain, double b_gain)
        {
            MMAL_PARAMETER_AWB_GAINS_T awb_gains = new MMAL_PARAMETER_AWB_GAINS_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_CUSTOM_AWB_GAINS, (uint)Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T(0,0), new MMAL_RATIONAL_T(0, 0));

            awb_gains.rGain.num = (int)(r_gain * 65536);
            awb_gains.bGain.num = (int)(b_gain * 65536);
            awb_gains.rGain.den = 65536;
            awb_gains.bGain.den = 65536;

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &awb_gains.hdr), "Unable to set awb gains");
        }
        
        public static void SetImageFx(this MMALPortBase port, MMAL_PARAM_IMAGEFX_T imageFx)
        {
            MMAL_PARAMETER_IMAGEFX_T imgFX = new MMAL_PARAMETER_IMAGEFX_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_IMAGE_EFFECT, (uint)Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                                                                                                        imageFx);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &imgFX.hdr), "Unable to set image fx");
        }

        public static void SetColourFx(this MMALPortBase port, ColourEffects colourFx)
        {
            MMAL_PARAMETER_COLOURFX_T colFX = new MMAL_PARAMETER_COLOURFX_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_COLOUR_EFFECT, (uint)Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0, 0, 0);

            colFX.enable = colourFx.Enable;
            colFX.u = colourFx.U;
            colFX.v = colourFx.V;

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &colFX.hdr), "Unable to set colour fx");
        }

        public static void SetRotation(this MMALPortBase port, int rotation)
        {
            int rot = ((rotation % 360) / 90) * 90;

            SetParameter(MMAL_PARAMETER_ROTATION, rot, port.Ptr);

        }

        public static void SetFlips(this MMALPortBase port, bool hflip, bool vflip)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_MIRROR, (uint)Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                                                                                                        MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            if (hflip && vflip)
                mirror.value = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH;
            else if (hflip)
                mirror.value = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_HORIZONTAL;
            else if (vflip)
                mirror.value = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL;

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &mirror.hdr), "Unable to set flips");

        }

        public static void SetROI(this MMALPortBase port, MMAL_RECT_T rect)
        {
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_INPUT_CROP, (uint)Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()), rect);

            crop.rect.x = (65536 * rect.x);
            crop.rect.y = (65536 * rect.y);
            crop.rect.width = (65536 * rect.width);
            crop.rect.height = (65536 * rect.height);
            
            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &crop.hdr), "Unable to set ROI");

        }

        public static void SetShutterSpeed(this MMALPortBase port, int speed)
        {
            SetParameter(MMAL_PARAMETER_SHUTTER_SPEED, speed, port.Ptr);
        }

        public static void SetDRC(this MMALPortBase port, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(new MMAL_PARAMETER_HEADER_T((uint)MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, (uint)Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                                                                                                        strength);
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &drc.hdr), "Unable to set DRC");
        }

        public static void SetStatsPass(this MMALPortBase port, int statsPass)
        {
            SetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS, statsPass, port.Ptr);
        }


    }
}
