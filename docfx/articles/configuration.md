# Configuration

## Debug Mode

When in debug mode, MMALSharp will print messages to the console output during image capture. The messages are helpful to indicate any potential issues 
during capture. 

`MMALCameraConfig.Debug = true;`

If further debugging is required, you can enable the native MMAL debugging logs as seen below:

1. In the /boot/config.txt file, add the following: `start_debug=1`
2. Prior to running MMALSharp, run `export VC_LOGLEVEL=mmal:trace`
3. Capture image using MMALSharp
4. Run `sudo vcdbg log msg` and `sudo vcdbg log assert

## Resolution

Changes the resolution of outputted images.

### Image Resolution

`MMALCameraConfig.StillResolution = new Resolution(1920, 1080);`

or

`MMALCameraConfig.StillResolution = Resolution.As5MPixel;`

### Video Resolution 

`MMALCameraConfig.VideoResolution = new Resolution(1920, 1080);`

or 

`MMALCameraConfig.VideoResolution = Resolution.As1080p;`

### Preview Resolution 

When recording Video, the Preview resolution must be the same as the Resolution of the recording video - this is automatically applied by MMALSharp if needed.
Does not apply to stills capture.

`MMALCameraConfig.PreviewResolution = new Resolution(1920, 1080);`

or 

`MMALCameraConfig.PreviewResolution = Resolution.As1080p;`


## Sharpness

Changes the Sharpness of an image.

Valid values: `0 - 100`

`MMALCameraConfig.Sharpness = 50;`

## Contrast

Changes the Contrast of an image.

Valid values: `0 - 100`

`MMALCameraConfig.Contrast = 50;`

## Brightness

Changes the Brightness of an image.

Valid values: `0 - 100`

`MMALCameraConfig.Brightness = 50;`

## Saturation

Changes the Saturation of an image.

Valid values: `0 - 100`

`MMALCameraConfig.Saturation = 50;`

## ISO

Changes the ISO setting used by the sensor. Relates to the amount of time the sensor is exposed to light. A lower value means 
the sensor will be exposed for longer. The Shutter Speed will automatically adjust based on the ISO value committed.

Valid values: `100 - 800`

`MMALCameraConfig.ISO = 200;`

## Exposure Compensation

Change the Exposure Compensation of the sensor - doing so will produce a lighter/darker image beyond the recommended exposure.

`MMALCameraConfig.ExposureCompensation = 0`

## Exposure Mode

Configure the Exposure Mode used by the sensor.

Valid values:

```
MMAL_PARAM_EXPOSUREMODE_AUTO, // auto: use automatic exposure mode
MMAL_PARAM_EXPOSUREMODE_NIGHT, // night: select setting for night shooting
MMAL_PARAM_EXPOSUREMODE_NIGHTPREVIEW,
MMAL_PARAM_EXPOSUREMODE_BACKLIGHT, // backlight: select setting for backlit subject
MMAL_PARAM_EXPOSUREMODE_SPOTLIGHT,
MMAL_PARAM_EXPOSUREMODE_SPORTS, // sports: select setting for sports(fast shutter etc.)
MMAL_PARAM_EXPOSUREMODE_SNOW, // snow: select setting optimised for snowy scenery
MMAL_PARAM_EXPOSUREMODE_BEACH, // beach: select setting optimised for beach
MMAL_PARAM_EXPOSUREMODE_VERYLONG, // verylong: select setting for long exposures
MMAL_PARAM_EXPOSUREMODE_FIXEDFPS, // fixedfps: constrain fps to a fixed value
MMAL_PARAM_EXPOSUREMODE_ANTISHAKE, // antishake: antishake mode
MMAL_PARAM_EXPOSUREMODE_FIREWORKS // fireworks: select setting optimised for fireworks
```

`MMALCameraConfig.ExposureCompensation = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;`


## Exposure Metering Mode

Configure the exposure metering mode to be used by the camera. The metering mode determines how the camera measures exposure.
 
Spot metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_SPOT):
 
With spot metering, the camera will only measure a very small area of the scene and ignores everything else.
On the Raspberry Pi camera, this will be the very centre of the image. 
 
Average metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE):

Using this metering mode, the camera will use the light information coming from the entire scene. It does not focus on any particular
area of the scene.

Matrix metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX):
 
Matrix metering works by dividing the entire frame into multiple "zones" which are then analysed on an individual basis for light and dark tones.

 
Sources:
[https://photographylife.com/understanding-metering-modes](https://photographylife.com/understanding-metering-modes)
[https://en.wikipedia.org/wiki/Metering_mode#Spot_metering](https://en.wikipedia.org/wiki/Metering_mode#Spot_metering)

Valid values:

```
MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE,
MMAL_PARAM_EXPOSUREMETERINGMODE_SPOT,
MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT,
MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX
```

`MMALCameraConfig.ExposureMeterMode = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;`


## Automatic white balance mode

Configure the Auto White Balance to be used by the camera

Valid values:

```
MMAL_PARAM_AWBMODE_OFF, // off: turn off white balance calculation
MMAL_PARAM_AWBMODE_AUTO, // auto: automatic mode(default)
MMAL_PARAM_AWBMODE_SUNLIGHT, // sun: sunny mode(between 5000K and 6500K)
MMAL_PARAM_AWBMODE_CLOUDY, // cloud: cloudy mode(between 6500K and 12000K)
MMAL_PARAM_AWBMODE_SHADE, // shade: shade mode
MMAL_PARAM_AWBMODE_TUNGSTEN, // tungsten: tungsten lighting mode(between 2500K and 3500K)
MMAL_PARAM_AWBMODE_FLUORESCENT, // fluorescent: fluorescent lighting mode(between 2500K and 4500K)
MMAL_PARAM_AWBMODE_INCANDESCENT, // incandescent: incandescent lighting mode
MMAL_PARAM_AWBMODE_FLASH, // flash: flash mode
MMAL_PARAM_AWBMODE_HORIZON // horizon: horizon mode
```

`MMALCameraConfig.AwbMode = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;`

## Image effects

Apply effects to the resulting image. Some effects may not be applicable depending on the firmware version.

Valid values:

```
MMAL_PARAM_IMAGEFX_NONE, // none: no effect (default)
MMAL_PARAM_IMAGEFX_NEGATIVE, // negative: invert the image colours
MMAL_PARAM_IMAGEFX_SOLARIZE, // solarise: solarise the image
MMAL_PARAM_IMAGEFX_POSTERIZE, // posterise: posterise the image
MMAL_PARAM_IMAGEFX_WHITEBOARD, // whiteboard: whiteboard effect
MMAL_PARAM_IMAGEFX_BLACKBOARD, // blackboard: blackboard effect
MMAL_PARAM_IMAGEFX_SKETCH, // sketch: sketch effect
MMAL_PARAM_IMAGEFX_DENOISE, // denoise: denoise the image
MMAL_PARAM_IMAGEFX_EMBOSS, // emboss: emboss the image
MMAL_PARAM_IMAGEFX_OILPAINT, // oilpaint: oil paint effect
MMAL_PARAM_IMAGEFX_HATCH, // hatch: hatch sketch effect
MMAL_PARAM_IMAGEFX_GPEN, // gpen: graphite sketch effect
MMAL_PARAM_IMAGEFX_PASTEL, // pastel: pastel effect
MMAL_PARAM_IMAGEFX_WATERCOLOUR, // watercolour: watercolour effect
MMAL_PARAM_IMAGEFX_FILM, // film: film grain effect
MMAL_PARAM_IMAGEFX_BLUR, // blur: blur the image
MMAL_PARAM_IMAGEFX_SATURATION, // saturation: colour saturate the image
MMAL_PARAM_IMAGEFX_COLOURSWAP, // colourswap: not fully implemented
MMAL_PARAM_IMAGEFX_WASHEDOUT, // washedout: not fully implemented
MMAL_PARAM_IMAGEFX_COLOURPOINT, // colourpoint: not fully implemented
MMAL_PARAM_IMAGEFX_COLOURBALANCE, // colourbalance: not fully implemented
MMAL_PARAM_IMAGEFX_CARTOON // cartoon: not fully implemented
```

`MMALCameraConfig.ImageFx = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;`


## Colour effects

Allows a user to change the colour of an image (CIE 1960), e.g. U = 128, V = 128 will result in a greyscale (monochrome) image.

`MMALCameraConfig.ColourFx = new ColourEffects { Enable = true, U = 128, V = 128 };`

## Rotation

Rotate the resulting image - possible values are 0, 90, 180, 270

`MMALCameraConfig.Rotation = 90;`

## Flip image

Flip the resulting image.

Valid values:

```
MMAL_PARAM_MIRROR_NONE,
MMAL_PARAM_MIRROR_VERTICAL,
MMAL_PARAM_MIRROR_HORIZONTAL,
MMAL_PARAM_MIRROR_BOTH
```

`MMALCameraConfig.Flips = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL;`


## Zoom (Region of interest)

Zoom in on the resulting image to produce a Region of Interest.

X, Y, Height and Width parameters must be less than 1.0.

`MMALCameraConfig.ROI = new Zoom { X = 0.5, Y = 0.5, Height = 0.1, Width = 0.1 };`

## Shutter speed

Adjust the shutter speed, this setting adjusts the length of time that the sensor is exposed to light. A fast shutter speed will reduce the length of time
it is exposed to light. 

There is an upper limit of 6000000us (6000ms, 6s), past which operation is undefined.

`MMALCameraConfig.ShutterSpeed = 1000000;`

## Automatic white balance - Red gains

Sets red AWB gains to be applied. Only applies when AwbMode is disabled.

`MMALCameraConfig.AwbGainsR = 2;`

## Automatic white balance - Blue gains

Sets blue AWB gains to be applied. Only applies when AwbMode is disabled.

`MMALCameraConfig.AwbGainsB = 2;`

## Dynamic range compression

Dynamic range compression increases the range of dark areas and decreases brighter areas, which helps improve the resulting image in low light areas.

Valid values:

```
MMAL_PARAMETER_DRC_STRENGTH_OFF, 
MMAL_PARAMETER_DRC_STRENGTH_LOW,
MMAL_PARAMETER_DRC_STRENGTH_MEDIUM,
MMAL_PARAMETER_DRC_STRENGTH_HIGH,
```

`MMALCameraConfig.DrcLevel = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_MEDIUM;`

## Statistics pass

Displays the exposure, analogue and digital gains, and AWB settings used.

`MMALCameraConfig.StatsPass = true;`

## Annotation

Allows annotation to be applied to the resulting image.

**Enable annotation**

`MMALCameraConfig.EnableAnnotate = true;`

**Customise annotation options**

`MMALCameraConfig.Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true };`

## Encoding

In MMALSharp, Components provide the ability to have their encoding type and pixel format changed. 

A user is able to change the encoding type and pixel format used by a Component by using one of the Encoding formats available in the `MMALSharp.Native.MMALEncoding` class.

In order to change the encoding type and pixel format for the Camera component, a user can alter the properties seen below:


```
MMALCameraConfig.StillEncoding
MMALCameraConfig.StillSubFormat

MMALCameraConfig.VideoEncoding
MMALCameraConfig.VideoSubformat

MMALCameraConfig.PreviewEncoding
MMALCameraConfig.PreviewSubformat
```

For `MMALEncoderBase` inheritors, the encoding type and pixel format are specified when the instances are constructed. 


# Video specific configuration

## Video stabilisation

Enables video stabilisation support when recording video

`MMALCameraConfig.VideoStabilisation = true;`

## Rate control

Not supported by firmware however code present.

## Intra refresh period (GoP)

**H.264 encoding only**

Every intra refresh period, H.264 video uses a complete frame (I-frame) which subsequent frames are then based upon. This setting specifies the number of frames between each I-frame. A higher value will reduce the size of the resulting video, and
a smaller value will result in a less error prone stream.

`MMALCameraConfig.IntraPeriod = 1;`

## Video profile

Sets the encoding profile. 

Valid values: See `MMALSharp.Native.MMALParametersVideo.MMAL_VIDEO_PROFILE_T`

`MMALCameraConfig.MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH;`

## Video level

Sets the encoding level.

Valid values: See `MMALSharp.Native.MMALParametersVideo.MMAL_VIDEO_LEVEL_T`

`MMALCameraConfig.MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;`

## Inline headers

When enabled, the stream will include PPS and SPS headers on every I-frame. Certain
streaming methods require this to be enabled e.g. Apple HLS.

`MMALCameraConfig.InlineHeaders = true;`

## Inline Motion Vectors

When enabled, Inline Motion Vector headers will be produced. These Vectors display
motion occurred between frames.

`MMALCameraConfig.InlineMotionVectors = true;`