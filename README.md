# MMALSharp

MMALSharp is an unofficial C# API for the Raspberry Pi camera. It is currently an early experimental build which features the ability to 
take pictures with your Raspberry Pi.

##Building

The project is currently targeting .NET framework 4.5.2, and therefore requires Mono 4.x.

MMALSharp uses FAKE for building, and Paket for dependency management. Currently there is only a dependency on the Nito.AsyncEx library by 
[@StephenCleary](https://github.com/StephenCleary).

To build, simply run one of the below depending on your development environment.

Windows:

`build.cmd`

Unix:

`dos2unix ./build.sh`

`sudo ./build.sh`

Once the library has built, you can reference it as a project within your application.

##Basic Usage

Using the library is relatively simple. Initially, you are required to create an instance of the `MMALCameraConfig` class, changing any 
properties you require (default values are set automatically on your behalf). Next, you should create an instance of the
`MMALCamera` class - this class is the main object to work with in the library which grants access to the variety of `TakePicture` methods available.

MMALSharp is asynchronous in nature, preventing any blocking of the main thread in your application. From testing, I found it is important that we provide a context
for the asynchronous code to run in, this is because when we await processing to complete, we need to return to the same thread we began processing on.

Below is a basic example of its usage.

```

public static void Main(string[] args)
{
        MMALCameraConfig config = new MMALCameraConfig
        {
            Sharpness = 100,
            Contrast = 10,
            ImageEffect = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NEGATIVE,
			EnableAnnotate = true,
			Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true }
        };

        using (MMALCamera cam = new MMALCamera(config))
		{
			AsyncContext.Run(async () =>
			{
				await cam.ConfigureCamera().TakePicture(new FileCaptureHandler("/home/pi/test3.jpg"), MMALEncodings.MMAL_ENCODING_JPEG, 90);
			});
		}
}

```

##Status

This is currently an experimental build and therefore a lot of functionality is not fully tested, however
a number of common features are working correctly.

The library has currently been tested on the following Raspberry Pi devices:

* Raspberry Pi 1 Model B (512mb)
* Raspberry Pi Zero

Both the SUNNY and Sony IMX219 camera modules are currently working as expected.


##License

MIT license 

Copyright (c) 2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

##Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.
