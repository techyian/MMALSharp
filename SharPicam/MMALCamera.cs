using SharPicam.Components;
using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public class MMALCamera
    {
        public MMALCameraComponent Camera { get; set; }


        public int Sharpness { get; set; } = 0;
        public int Contrast { get; set; } = 0;
        public int Brightness { get; set; } = 50;
        public int Saturation { get; set; } = 0;
        public int ISO { get; set; } = 0;
        public bool VideoStabilisation { get; set; } = true;
        public int ExposureCompensation { get; set; } = (int)MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
        public MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
        public MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;
        public MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;
        public MMAL_PARAM_IMAGEFX_T ImageEffect { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;
        public ColourEffects Effects { get; set; } = new ColourEffects();
        public int Rotation { get; set; } = 0;
        public int HFlip { get; set; } = 0;
        public int VFlip { get; set; } = 0;
        public ROI ROI { get; set; } = new ROI();
        public int ShutterSpeed { get; set; } = 0;
        public int AwbGainsR { get; set; } = 0;
        public int AwbGainsB { get; set; } = 0;
        public MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;
        public int StatsPass { get; set; } = 0;
        public int EnableAnnotate { get; set; } = 0;
        public int AnnotateTextSize { get; set; } = 0;
        public int AnnotateTextColour { get; set; } = -1;
        public int AnnotateBgColour { get; set; } = -1;
        public StereoMode StereoMode { get; set; } = new StereoMode();

        public MMALCamera()
        {
            BcmHost.bcm_host_init();
            this.Camera = new MMALCameraComponent();


        }


    }
}
