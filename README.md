# MMALSharp

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp)

MMALSharp is an unofficial C# API for the Raspberry Pi camera. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

The project is in early stages of development, however the ability to take pictures and record video are working as expected (H.264 & MJPEG).

MMALSharp supports the following runtimes:

1) Mono 4.x 
2) .NET Core 2.0 (beta) with .NET Standard 1.6.


## Mono

### Installation

In order for the Raspberry Pi to run C# programs, you will need to install Mono. Installation differs between the original Model A/B/B+/Zero boards and
the newer Pi Model B 2/3 boards running the ARMV7/8 chipsets.

#### Model A/B/B+/Zero

The version of Mono currently available in the Raspbian repositories is 3.2.8 and isn't compatible with this library. Therefore, we need to do a few
extra steps to get a compatible version installed. Luckily, member 'plugwash' from the Raspberry Pi forums has built a version of Mono and provided a
repository from which we can install.

In order to install the required version, please open a console window and follow the below steps:

1. Run `sudo nano /etc/apt/sources.list`
2. On a new line, enter `deb http://plugwash.raspbian.org/mono4 wheezy-mono4 main`
3. Run `sudo apt-get update && sudo apt-get upgrade`
4. Run `sudo apt-get install mono-complete`

Once completed, if you run `mono --version` from your command window, you should see the mono version 4.0.2 returned.

#### Model B 2/3

Using a later model of the Raspberry Pi allows you to install the latest Mono version from the Mono repositories without issue. To do so, please follow the below steps:

```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update && sudo apt-get upgrade
sudo apt-get install mono-complete
```

## .NET Core

.NET Core support for the Raspberry Pi is available in .NET Core 2.0 (beta), however only the Pi 2 & 3 are currently supported and you must be using an Ubuntu flavour distro; for local testing I am using Ubuntu MATE 16.04 (LTS).
 
### Installation
 
1) Download the .NET Core SDK v2.0 from [here](https://github.com/dotnet/cli) - scroll down to the 'Installers and Binaries' section and download & install the appropriate binaries.
2) Install the following packages on your Raspberry Pi: `sudo apt-get install libunwind8 libunwind8-dev gettext libicu-dev liblttng-ust-dev libcurl4-openssl-dev libssl-dev uuid-dev unzip`
3) Clone MMALSharp
4) Enter the `.paket` directory and run paket.bootstrapper.exe
5) When complete, a new executable `paket.exe` can be found in the same directory - run `paket install` which will install all dependencies required for MMALSharp.
6) Change directory back to the root solution level `cd ..`
7) Run `dotnet restore` which will configure the .NET Core projects.
8) Run `dotnet publish -r ubuntu.16.04-arm` - this will create a new directory called `publish` within the `/src/MMALSharpCoreExample/bin/Debug/netcoreapp2.0/ directory.
9) Copy the contents of that folder over to your Raspberry Pi
10) Download and extract the .NET Core runtime on your Pi from [here](https://github.com/dotnet/core-setup#daily-builds), ensuring you choose the correct distribution ([Ubuntu 16.04 download location](https://dotnetcli.blob.core.windows.net/dotnet/master/Binaries/Latest/dotnet-ubuntu.16.04-arm.latest.tar.gz) )
11) Within the extracted directory will be an application called `dotnet`, run `sudo chmod +x ./dotnet` to make it executable, then run `dotnet LOCATION OF YOUR MMALSharpCoreExample.dll`.


## Building from source

**These build instructions are specifically for running MMALSharp with Mono:**

Pre-release builds are available from [Myget](https://www.myget.org/gallery/mmalsharp)

Windows:

`build.cmd`

Unix:

```
sudo apt-get install dos2unix
sudo chmod +x ./build.sh
dos2unix ./build.sh
./build.sh
```

Once the library has built, you can reference it as a project within your application.

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
			await cam.TakePicture(cam.Camera.StillPort, cam.Camera.StillPort);
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
