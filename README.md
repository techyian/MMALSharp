# MMALSharp

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp)

MMALSharp is an unofficial C# API for the Raspberry Pi camera. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

The project is in early stages of development, however the ability to take pictures and record video is working as expected (H.264 & MJPEG).

MMALSharp supports the following runtimes:

1) Mono 4.x 
2) .NET Core 2.0 (beta) with .NET Standard 1.6.

## Installation

For full installation instructions for Mono 4.x and .NET Core - please visit the [Documentation](https://techyian.github.io/MMALSharp) site

## Basic Usage

Using the library is relatively simple. If you want to change any of the default configuration settings, this can be done by modifying the 
properties within `MMALCameraConfig`. The main class `MMALCamera` which interfaces to the rest of the functionality the library provides is 
a Singleton and is called as follows: `MMALCamera cam = MMALCamera.Instance`.

MMALSharp is asynchronous in nature, preventing any blocking of the main thread in your application. From testing, I found it is important that we provide a context
for the asynchronous code to run in, this is because when we await processing to complete, we need to return to the same thread we began processing on.

Below is a basic example of its usage.

```

public static void Main(string[] args)
{
    Alter any configuration properties required.         
    MMALCameraConfig.EnableAnnotate = true;
    MMALCameraConfig.Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true };
	MMALCameraConfig.VideoHeight = 1024;
    MMALCameraConfig.VideoWidth = 768;
		
	//Required for segmented recording
	MMALCameraConfig.InlineHeaders = true;
		
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		using (var vidEncoder = new MMALVideoEncoder(new VideoStreamCaptureHandler("/home/pi/videos", ".avi"), 40))
		using (var imgEncoder = new MMALImageEncoder(new ImageStreamCaptureHandler("/home/pi/images/", "jpg")))
		{
			//Create our component pipeline.         
			cam.AddEncoder(vidEncoder, cam.Camera.VideoPort)
			   .AddEncoder(imgEncoder, cam.Camera.StillPort)
			   .CreatePreviewComponent(new MMALVideoRenderer())
			   .ConfigureCamera();

			//Record video for 1 minute, using segmented video record to split into multiple files every 30 seconds.
			await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1), new Split { Mode = TimelapseMode.Second, Value = 30 });

			//Take a single picture on the camera's still port using the encoder connected to the still port
			await cam.TakePicture(cam.Camera.StillPort);
		}
						
		//Once we're finished with the camera and will *not* use it again, cleanup any unmanaged resources.
		cam.Cleanup();                
	});		
}

```

## Status

So far, the library has been tested on the following Raspberry Pi devices:

* Raspberry Pi 1 Model B (512mb)
* Raspberry Pi Zero
* Raspberry Pi 2 Model B

Both the SUNNY and Sony IMX219 camera modules are working as expected.

Tested image 'still' features:

- [x] Image width/height
- [x] Image encoding
- [x] Brightness
- [x] Contrast
- [x] Saturation
- [x] Sharpness
- [x] Shutter speed
- [x] ISO
- [x] Exposure compensation
- [x] Exposure mode
- [x] Exposure metering mode
- [x] Raspistill supported image effects
- [x] Rotation
- [x] Flips
- [x] Annotate
- [x] Dynamic range compression
- [x] Stats Pass
- [x] Colour effects
- [x] Crop
- [x] Auto white balance mode/gains
- [x] EXIF tags
- [x] Raw capture

## Notes & Known issues

When using more resource intensive encoders such as MMAL_ENCODING_BMP and the Sony IMX219 module, I've found it necessary to increase the memory split
to around 200mb or otherwise you'll receive an ENOSPC error due to insufficient resources.

There is an issue with EXIF and Annotation support under the .NET Core build of MMALSharp currently, an issue has been raised for this and will be fixed ASAP.

## License

MIT license 

Copyright (c) 2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

## Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.
