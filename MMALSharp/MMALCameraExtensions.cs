using MMALSharp.Components;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALCallerHelper;
using static MMALSharp.MMALParameterHelpers;
using static MMALSharp.Native.MMALParameters;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp
{
    internal unsafe static class MMALCameraComponentExtensions
    {
        public static void SetCameraConfig(this MMALCameraComponent camera, MMAL_PARAMETER_CAMERA_CONFIG_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &value.hdr), "Unable to set camera config.");
        }

        public static void SetChangeEventRequest(this MMALControlPortImpl controlPort, MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(controlPort.Ptr, &value.hdr), "Unable to set camera event request.");
        }

        public static double GetSaturation(this MMALCamera camera)
        {            
            return GetParameter(MMAL_PARAMETER_SATURATION, camera.Camera.Control.Ptr);            
        }

        public static void SetSaturation(this MMALCamera camera, double saturation)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting saturation: {0}", saturation));

            var value = new MMAL_RATIONAL_T((int)saturation, 100);

            if (saturation >= -100 && saturation <= 100)
            {
                SetParameter(MMAL_PARAMETER_SATURATION, value, camera.Camera.Control.Ptr);
            }
            else
            {
                throw new Exception("Invalid saturation value");
            }
                
        }

        public static double GetSharpness(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_SHARPNESS, camera.Camera.Control.Ptr);                        
        }

        public static void SetSharpness(this MMALCamera camera, double sharpness)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting sharpness: {0}", sharpness));

            var value = new MMAL_RATIONAL_T((int)sharpness, 100);

            if (sharpness >= -100 && sharpness <= 100)
            {
                SetParameter(MMAL_PARAMETER_SHARPNESS, value, camera.Camera.Control.Ptr);
            }
            else
            {
                throw new Exception("Invalid sharpness value");
            }                        
        }

        public static double GetContrast(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_CONTRAST, camera.Camera.Control.Ptr);                        
        }

        public static void SetContrast(this MMALCamera camera, double contrast)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting contrast: {0}", contrast));

            var value = new MMAL_RATIONAL_T((int)contrast, 100);

            if (contrast >= -100 && contrast <= 100)
            {
                SetParameter(MMAL_PARAMETER_CONTRAST, value, camera.Camera.Control.Ptr);
            }
            else
            {
                throw new Exception("Invalid contrast value");
            }
                     
        }

        public static void SetDisableExif(this MMALImageEncoder encoder, bool disable)
        {           
           SetParameter(MMAL_PARAMETER_EXIF_DISABLE, disable, encoder.Outputs.ElementAt(0).Ptr);           
        }

        public static double GetBrightness(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_BRIGHTNESS, camera.Camera.Control.Ptr);                        
        }

        public static void SetBrightness(this MMALCamera camera, double brightness)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting brightness: {0}", brightness));

            var value = new MMAL_RATIONAL_T((int)brightness, 100);

            if (brightness >= -100 && brightness <= 100)
            {
                SetParameter(MMAL_PARAMETER_BRIGHTNESS, value, camera.Camera.Control.Ptr);
            }
            else
            {
                throw new Exception("Invalid brightness value");
            }
            
        }

        public static int GetISO(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_ISO, camera.Camera.Control.Ptr);
        }

        public static void SetISO(this MMALCamera camera, int iso)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting ISO: {0}", iso));

            SetParameter(MMAL_PARAMETER_ISO, iso, camera.Camera.Control.Ptr);                        
        }

        public static bool GetVideoStabilisation(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_VIDEO_STABILISATION, camera.Camera.Control.Ptr);
        }

        public static void SetVideoStabilisation(this MMALCamera camera, bool vstabilisation)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting video stabilisation: {0}", vstabilisation));

            SetParameter(MMAL_PARAMETER_VIDEO_STABILISATION, vstabilisation, camera.Camera.Control.Ptr);            
        }

        public static int GetExposureCompensation(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_EXPOSURE_COMP, camera.Camera.Control.Ptr);
        }

        public static void SetExposureCompensation(this MMALCamera camera, int expCompensation)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting exposure compensation: {0}", expCompensation));

            SetParameter(MMAL_PARAMETER_EXPOSURE_COMP, expCompensation, camera.Camera.Control.Ptr);
        }

        public static MMAL_PARAM_EXPOSUREMODE_T GetExposureMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_EXPOSUREMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                                                                                                        new MMAL_PARAM_EXPOSUREMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &exp_mode.hdr), "Unable to get exposure mode");

            return exp_mode.value;                                    
        }

        public static void SetExposureMode(this MMALCamera camera, MMAL_PARAM_EXPOSUREMODE_T mode)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting exposure mode: {0}", mode));

            MMAL_PARAMETER_EXPOSUREMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &exp_mode.hdr), "Unable to set exposure mode");
        }

        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T GetExposureMeteringMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        new MMAL_PARAM_EXPOSUREMETERINGMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &exp_mode.hdr), "Unable to get exposure metering mode");

            return exp_mode.value;
        }

        public static void SetExposureMeteringMode(this MMALCamera camera, MMAL_PARAM_EXPOSUREMETERINGMODE_T mode)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting exposure metering mode: {0}", mode));

            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T exp_mode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &exp_mode.hdr), "Unable to set exposure metering mode");
        }

        public static MMAL_PARAM_AWBMODE_T GetAwbMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_AWBMODE_T awb_mode = new MMAL_PARAMETER_AWBMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        new MMAL_PARAM_AWBMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &awb_mode.hdr), "Unable to get awb mode");

            return awb_mode.value;
        }

        public static void SetAwbMode(this MMALCamera camera, MMAL_PARAM_AWBMODE_T mode)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting AWB mode: {0}", mode));

            MMAL_PARAMETER_AWBMODE_T awb_mode = new MMAL_PARAMETER_AWBMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &awb_mode.hdr), "Unable to set awb mode");
        }

        public static Tuple<double, double> GetAwbGains(this MMALCamera camera)
        {
            MMAL_PARAMETER_AWB_GAINS_T awb_gains = new MMAL_PARAMETER_AWB_GAINS_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CUSTOM_AWB_GAINS, Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &awb_gains.hdr), "Unable to get awb gains");

            MMAL_RATIONAL_T rGain = awb_gains.rGain;
            MMAL_RATIONAL_T bGain = awb_gains.bGain;

            double r = rGain.num / rGain.den;
            double b = bGain.num / bGain.den;

            return new Tuple<double, double>(r, b);
        }

        public static void SetAwbGains(this MMALCamera camera, double r_gain, double b_gain)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting AWB gains: {0}, {1}", r_gain, b_gain));

            MMAL_PARAMETER_AWB_GAINS_T awb_gains = new MMAL_PARAMETER_AWB_GAINS_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CUSTOM_AWB_GAINS, Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0));

            awb_gains.rGain.num = (int)(r_gain * 65536);
            awb_gains.bGain.num = (int)(b_gain * 65536);
            awb_gains.rGain.den = 65536;
            awb_gains.bGain.den = 65536;

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &awb_gains.hdr), "Unable to set awb gains");
        }

        public static MMAL_PARAM_IMAGEFX_T GetImageFx(this MMALCamera camera)
        {
            MMAL_PARAMETER_IMAGEFX_T imgFX = new MMAL_PARAMETER_IMAGEFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                                                                                                        new MMAL_PARAM_IMAGEFX_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &imgFX.hdr), "Unable to get image fx");

            return imgFX.value;
        }

        public static void SetImageFx(this MMALCamera camera, MMAL_PARAM_IMAGEFX_T imageFx)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting Image FX: {0}", imageFx));

            MMAL_PARAMETER_IMAGEFX_T imgFX = new MMAL_PARAMETER_IMAGEFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                                                                                                        imageFx);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &imgFX.hdr), "Unable to set image fx");
        }

        public static ColourEffects GetColourFx(this MMALCamera camera)
        {
            MMAL_PARAMETER_COLOURFX_T colFX = new MMAL_PARAMETER_COLOURFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0, 0, 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &colFX.hdr), "Unable to get colour fx");

            ColourEffects fx = new ColourEffects();
            fx.Enable = colFX.enable;
            fx.U = colFX.u;
            fx.V = colFX.v;

            return fx;
        }

        public static void SetColourFx(this MMALCamera camera, ColourEffects colourFx)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting colour effects"));

            MMAL_PARAMETER_COLOURFX_T colFX = new MMAL_PARAMETER_COLOURFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0, 0, 0);

            colFX.enable = colourFx.Enable;
            colFX.u = colourFx.U;
            colFX.v = colourFx.V;

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &colFX.hdr), "Unable to set colour fx");
        }

        public static int GetRotation(this MMALCamera camera)
        {            
            return GetParameter(MMAL_PARAMETER_ROTATION, camera.Camera.StillPort.Ptr);
        }

        public static void SetRotation(this MMALCamera camera, int rotation)
        {            
            int rot = ((rotation % 360) / 90) * 90;

            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting rotation: {0}", rot));

            SetParameter(MMAL_PARAMETER_ROTATION, rot, camera.Camera.StillPort.Ptr);
        }

        public static MMAL_PARAM_MIRROR_T GetFlips(this MMALCamera camera)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                                                                                                        MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            
            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.StillPort.Ptr, &mirror.hdr), "Unable to get flips");

            return mirror.value;
        }

        public static void SetFlips(this MMALCamera camera, MMAL_PARAM_MIRROR_T flips)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                                                                                                        MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            mirror.value = flips;

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.StillPort.Ptr, &mirror.hdr), "Unable to set flips");

        }

        public static MMAL_RECT_T GetROI(this MMALCamera camera)
        {
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()), new MMAL_RECT_T());
                        
            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &crop.hdr), "Unable to get ROI");

            return crop.rect;
        }

        public static void SetROI(this MMALCamera camera, MMAL_RECT_T rect)
        {
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()), rect);

            crop.rect.x = (65536 * rect.x);
            crop.rect.y = (65536 * rect.y);
            crop.rect.width = (65536 * rect.width);
            crop.rect.height = (65536 * rect.height);
            
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &crop.hdr), "Unable to set ROI");

        }

        public static int GetShutterSpeed(this MMALCamera camera)
        {
            return GetParameter(MMAL_PARAMETER_SHUTTER_SPEED, camera.Camera.Control.Ptr);
        }

        public static void SetShutterSpeed(this MMALCamera camera, int speed)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Setting shutter speed: {0}", speed));

            SetParameter(MMAL_PARAMETER_SHUTTER_SPEED, speed, camera.Camera.Control.Ptr);
        }

        public static MMAL_PARAMETER_DRC_STRENGTH_T GetDRC(this MMALCamera camera)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                                                                                                        new MMAL_PARAMETER_DRC_STRENGTH_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &drc.hdr), "Unable to get DRC");

            return drc.strength;
        }

        public static void SetDRC(this MMALCamera camera, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                                                                                                        strength);
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &drc.hdr), "Unable to set DRC");
        }

        public static void SetStatsPass(this MMALCamera camera, int statsPass)
        {
            SetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS, statsPass, camera.Camera.Control.Ptr);
        }

        public static void SetImageCapture(this MMALPortImpl port, bool enable)
        {
            SetParameter(MMAL_PARAMETER_CAPTURE, enable, port.Ptr);
        }

        public static void SetRawCapture(this MMALPortImpl port, bool raw)
        {
            SetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE, raw, port.Ptr);
        }

    }
}
