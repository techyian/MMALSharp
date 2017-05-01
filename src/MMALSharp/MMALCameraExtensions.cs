using MMALSharp.Components;
using MMALSharp.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using static MMALSharp.MMALCallerHelper;
using static MMALSharp.MMALParameterHelpers;
using static MMALSharp.Native.MMALParameters;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp
{
    public static unsafe class MMALCameraComponentExtensions
    {
        internal static void SetCameraConfig(this MMALCameraComponent camera, MMAL_PARAMETER_CAMERA_CONFIG_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &value.hdr), "Unable to set camera config.");
        }

        internal static void SetChangeEventRequest(this MMALControlPort controlPort, MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(controlPort.Ptr, &value.hdr), "Unable to set camera event request.");
        }

        public static double GetSaturation(this MMALCamera camera)
        {            
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_SATURATION);            
        }

        internal static void SetSaturation(this MMALCamera camera, double saturation)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting saturation: {saturation}");

            var value = new MMAL_RATIONAL_T((int)saturation, 100);

            if (saturation >= -100 && saturation <= 100)
            {
                camera.Camera.Control.SetParameter(MMAL_PARAMETER_SATURATION, value);
            }
            else
            {
                throw new Exception("Invalid saturation value");
            }                
        }

        public static double GetSharpness(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_SHARPNESS);                        
        }

        internal static void SetSharpness(this MMALCamera camera, double sharpness)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting sharpness: {sharpness}");

            var value = new MMAL_RATIONAL_T((int)sharpness, 100);

            if (sharpness >= -100 && sharpness <= 100)
            {
                camera.Camera.Control.SetParameter(MMAL_PARAMETER_SHARPNESS, value);
            }
            else
            {
                throw new Exception("Invalid sharpness value");
            }                        
        }

        public static double GetContrast(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_CONTRAST);                        
        }

        internal static void SetContrast(this MMALCamera camera, double contrast)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting contrast: {contrast}");

            var value = new MMAL_RATIONAL_T((int)contrast, 100);

            if (contrast >= -100 && contrast <= 100)
            {
                camera.Camera.Control.SetParameter(MMAL_PARAMETER_CONTRAST, value);
            }
            else
            {
                throw new Exception("Invalid contrast value");
            }
                     
        }

        internal static void SetDisableExif(this MMALImageEncoder encoder, bool disable)
        {
            encoder.Outputs.ElementAt(0).SetParameter(MMAL_PARAMETER_EXIF_DISABLE, disable);           
        }

        public static double GetBrightness(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_BRIGHTNESS);                        
        }

        internal static void SetBrightness(this MMALCamera camera, double brightness)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting brightness: {brightness}");

            var value = new MMAL_RATIONAL_T((int)brightness, 100);

            if (brightness >= 0 && brightness <= 100)
            {
                camera.Camera.Control.SetParameter(MMAL_PARAMETER_BRIGHTNESS, value);
            }
            else
            {
                throw new Exception("Invalid brightness value");
            }
            
        }

        public static int GetISO(this MMALCamera camera)
        {
            return (int)camera.Camera.Control.GetParameter(MMAL_PARAMETER_ISO);
        }

        internal static void SetISO(this MMALCamera camera, int iso)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting ISO: {iso}");

            //0 = auto
            if ((iso < 100 || iso > 800) && iso > 0)
            {
                throw new PiCameraError("Invalid ISO setting. Valid values: 100 - 800");
            }

            camera.Camera.Control.SetParameter(MMAL_PARAMETER_ISO, iso);                        
        }

        public static bool GetVideoStabilisation(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_VIDEO_STABILISATION);
        }

        internal static void SetVideoStabilisation(this MMALCamera camera, bool vstabilisation)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting video stabilisation: {vstabilisation}");

            camera.Camera.Control.SetParameter(MMAL_PARAMETER_VIDEO_STABILISATION, vstabilisation);            
        }

        public static int GetExposureCompensation(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_EXPOSURE_COMP);
        }

        internal static void SetExposureCompensation(this MMALCamera camera, int expCompensation)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting exposure compensation: {expCompensation}");

            if (expCompensation < -10 || expCompensation > 10)
            {
                throw new PiCameraError("Invalid exposure compensation value. Valid values (-10 - 10)");
            }

            camera.Camera.Control.SetParameter(MMAL_PARAMETER_EXPOSURE_COMP, expCompensation);
        }

        public static MMAL_PARAM_EXPOSUREMODE_T GetExposureMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_EXPOSUREMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                                                                                                        new MMAL_PARAM_EXPOSUREMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &expMode.hdr), "Unable to get exposure mode");

            return expMode.Value;                                    
        }

        internal static void SetExposureMode(this MMALCamera camera, MMAL_PARAM_EXPOSUREMODE_T mode)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting exposure mode: {mode}");

            MMAL_PARAMETER_EXPOSUREMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &expMode.hdr), "Unable to set exposure mode");
        }

        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T GetExposureMeteringMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        new MMAL_PARAM_EXPOSUREMETERINGMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &expMode.hdr), "Unable to get exposure metering mode");

            return expMode.Value;
        }

        internal static void SetExposureMeteringMode(this MMALCamera camera, MMAL_PARAM_EXPOSUREMETERINGMODE_T mode)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting exposure metering mode: {mode}");

            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &expMode.hdr), "Unable to set exposure metering mode");
        }

        public static MMAL_PARAM_AWBMODE_T GetAwbMode(this MMALCamera camera)
        {
            MMAL_PARAMETER_AWBMODE_T awbMode = new MMAL_PARAMETER_AWBMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        new MMAL_PARAM_AWBMODE_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &awbMode.hdr), "Unable to get awb mode");

            return awbMode.Value;
        }

        internal static void SetAwbMode(this MMALCamera camera, MMAL_PARAM_AWBMODE_T mode)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting AWB mode: {mode}");

            MMAL_PARAMETER_AWBMODE_T awbMode = new MMAL_PARAMETER_AWBMODE_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &awbMode.hdr), "Unable to set awb mode");
        }

        public static MMAL_PARAMETER_CAMERA_SETTINGS_T GetCameraSettings(this MMALCamera camera)
        {
            MMAL_PARAMETER_CAMERA_SETTINGS_T settings = new MMAL_PARAMETER_CAMERA_SETTINGS_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CAMERA_SETTINGS, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_SETTINGS_T>()),
                                                                                             0, new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 
                                                                                             new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &settings.hdr), "Unable to get camera settings");
            
            return settings;
        }

        public static Tuple<double, double> GetAwbGains(this MMALCamera camera)
        {
            MMAL_PARAMETER_AWB_GAINS_T awbGains = new MMAL_PARAMETER_AWB_GAINS_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CUSTOM_AWB_GAINS, Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T(0, 65536), new MMAL_RATIONAL_T(0, 65536));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &awbGains.hdr), "Unable to get awb gains");

            MMAL_RATIONAL_T rGain = awbGains.RGain;
            MMAL_RATIONAL_T bGain = awbGains.BGain;

            double r = rGain.Num / rGain.Den;
            double b = bGain.Num / bGain.Den;

            return new Tuple<double, double>(r, b);
        }

        internal static void SetAwbGains(this MMALCamera camera, double rGain, double bGain)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting AWB gains: {rGain}, {bGain}");

            if (MMALCameraConfig.AwbMode != MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_OFF && (rGain > 0 || bGain > 0))
                throw new PiCameraError("AWB Mode must be off when setting AWB gains");
            
            MMAL_PARAMETER_AWB_GAINS_T awbGains = new MMAL_PARAMETER_AWB_GAINS_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CUSTOM_AWB_GAINS, Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T((int)(rGain * 65536), 65536), 
                                                                                                        new MMAL_RATIONAL_T((int)(bGain * 65536), 65536));
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &awbGains.hdr), "Unable to set awb gains");
        }

        public static MMAL_PARAM_IMAGEFX_T GetImageFx(this MMALCamera camera)
        {
            MMAL_PARAMETER_IMAGEFX_T imgFx = new MMAL_PARAMETER_IMAGEFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                                                                                                        new MMAL_PARAM_IMAGEFX_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &imgFx.hdr), "Unable to get image fx");

            return imgFx.Value;
        }

        internal static void SetImageFx(this MMALCamera camera, MMAL_PARAM_IMAGEFX_T imageFx)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting Image FX: {imageFx}");

            MMAL_PARAMETER_IMAGEFX_T imgFx = new MMAL_PARAMETER_IMAGEFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                imageFx);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &imgFx.hdr), "Unable to set image fx");
        }

        public static ColourEffects GetColourFx(this MMALCamera camera)
        {
            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0, 0, 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &colFx.hdr), "Unable to get colour fx");

            ColourEffects fx = new ColourEffects();
            fx.Enable = colFx.Enable == 1;
            fx.U = colFx.U;
            fx.V = colFx.V;

            return fx;
        }

        internal static void SetColourFx(this MMALCamera camera, ColourEffects colourFx)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine("Setting colour effects");

            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        (colourFx.Enable ? 1 : 0), colourFx.U, colourFx.V);
                    
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &colFx.hdr), "Unable to set colour fx");
        }

        public static int GetRotation(this MMALCamera camera)
        {            
            return camera.Camera.StillPort.GetParameter(MMAL_PARAMETER_ROTATION);
        }

        internal static void SetRotation(this MMALCamera camera, int rotation)
        {            
            int rot = ((rotation % 360) / 90) * 90;

            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting rotation: {rot}");

            camera.Camera.StillPort.SetParameter(MMAL_PARAMETER_ROTATION, rot);
        }

        public static MMAL_PARAM_MIRROR_T GetFlips(this MMALCamera camera)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                                                                                                        MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            
            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.StillPort.Ptr, &mirror.hdr), "Unable to get flips");

            return mirror.Value;
        }

        internal static void SetFlips(this MMALCamera camera, MMAL_PARAM_MIRROR_T flips)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                                                                                                        flips);
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.StillPort.Ptr, &mirror.hdr), "Unable to set flips");

        }

        public static MMAL_RECT_T GetZoom(this MMALCamera camera)
        {
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()), new MMAL_RECT_T());
                        
            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &crop.hdr), "Unable to get zoom");

            return crop.Rect;
        }

        internal static void SetZoom(this MMALCamera camera, Zoom rect)
        {
            if (rect.X > 1.0 || rect.Y > 1.0 || rect.Height > 1.0 || rect.Width > 1.0)
                throw new PiCameraError("Invalid zoom settings. Value mustn't be greater than 1.0");
            
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()), 
                                                                                new MMAL_RECT_T(Convert.ToInt32(65536 * rect.X), Convert.ToInt32(65536 * rect.Y), Convert.ToInt32(65536 * rect.Width), Convert.ToInt32(65536 * rect.Height)));
                                    
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &crop.hdr), "Unable to set zoom");
        }

        public static int GetShutterSpeed(this MMALCamera camera)
        {
            return (int)camera.Camera.Control.GetParameter(MMAL_PARAMETER_SHUTTER_SPEED);
        }

        internal static void SetShutterSpeed(this MMALCamera camera, int speed)
        {
            if (MMALCameraConfig.Debug)
                Console.WriteLine($"Setting shutter speed: {speed}");

            if (speed > 6000000)
            {
                MMALSharp.Utility.Helpers.PrintWarning("Shutter speed exceeds upper supported limit of 6000ms. Undefined behaviour may result.");
            }

            camera.Camera.Control.SetParameter(MMAL_PARAMETER_SHUTTER_SPEED, speed);
        }

        public static MMAL_PARAMETER_DRC_STRENGTH_T GetDRC(this MMALCamera camera)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                                                                                                        new MMAL_PARAMETER_DRC_STRENGTH_T());

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Camera.Control.Ptr, &drc.hdr), "Unable to get DRC");

            return drc.Strength;
        }

        internal static void SetDRC(this MMALCamera camera, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                                                                                                        strength);
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Camera.Control.Ptr, &drc.hdr), "Unable to set DRC");
        }

        public static bool GetStatsPass(this MMALCamera camera)
        {
            return camera.Camera.Control.GetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS);
        }

        internal static void SetStatsPass(this MMALCamera camera, bool statsPass)
        {
            camera.Camera.Control.SetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS, statsPass);
        }

        internal static void SetImageCapture(this MMALPortImpl port, bool enable)
        {
            port.SetParameter(MMAL_PARAMETER_CAPTURE, enable);
        }

        public static bool GetRawCapture(this MMALPortImpl port)
        {
            return port.GetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE);
        }

        internal static void SetRawCapture(this MMALPortImpl port, bool raw)
        {
            port.SetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE, raw);
        }

        internal static void SetStereoMode(this MMALPortImpl port, StereoMode mode)
        {
            MMAL_PARAMETER_STEREOSCOPIC_MODE_T stereo = new MMAL_PARAMETER_STEREOSCOPIC_MODE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_STEREOSCOPIC_MODE, Marshal.SizeOf<MMAL_PARAMETER_STEREOSCOPIC_MODE_T>()),
                                                                                            mode.Mode, mode.Decimate, mode.SwapEyes);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &stereo.hdr), "Unable to set Stereo mode");
        }

    }
}
