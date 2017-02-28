# MMALSharp

MMALSharp is an unofficial C# API for the Raspberry Pi camera. It is currently an early experimental build which features the ability to 
take pictures with your Raspberry Pi. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

##Goals

My goal for the project is to provide a fully functional API to the Raspberry Pi camera, supporting Still image capture, Video capture and Preview rendering.
I aim to allow users the ability to build up their own component pipeline with respective encoders/decoders in an easy to use manner, adhering to what is 
possible via MMAL. 

##Install Mono

In order for the Raspberry Pi to run C# programs, you will need to install Mono. Installation differs between the original Model A/B/B+/Zero boards and
the newer Pi Model B 2/3 boards running the ARMV7/8 chipsets.

###Model A/B/B+/Zero

The version of Mono currently available in the Raspbian repositories is 3.2.8 and isn't compatible with this library. Therefore, we need to do a few
extra steps to get a compatible version installed. Luckily, member 'plugwash' from the Raspberry Pi forums has built a version of Mono and provided a
repository from which we can install.

In order to install the required version, please open a console window and follow the below steps:

1. Run `sudo nano /etc/apt/sources.list`
2. On a new line, enter `deb http://plugwash.raspbian.org/mono4 wheezy-mono4 main`
3. Run `sudo apt-get update && sudo apt-get upgrade`
4. Run `sudo apt-get install mono-complete`

Once completed, if you run `mono --version` from your command window, you should see the mono version 4.0.2 returned.

###Model B 2/3

Using a later model of the Raspberry Pi allows you to install the latest Mono version from the Mono repositories without issue. To do so, please follow the below steps:

```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update && sudo apt-get upgrade
sudo apt-get install mono-complete
```


##Building

The project is currently targeting .NET framework 4.5.2, and therefore requires Mono 4.x.

MMALSharp uses FAKE for building, and Paket for dependency management. Currently there is only a dependency on the Nito.AsyncEx library by 
[@StephenCleary](https://github.com/StephenCleary).

To build, simply run one of the below depending on your development environment.

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

##Basic Usage

Using the library is relatively simple. If you want to change any of the default configuration settings, this can be done by modifying the 
properties within `MMALCameraConfig`. The main class `MMALCamera` which interfaces to the rest of the functionality the library provides is 
a Singleton and is called as follows: `MMALCamera cam = MMALCamera.Instance`.

MMALSharp is asynchronous in nature, preventing any blocking of the main thread in your application. From testing, I found it is important that we provide a context
for the asynchronous code to run in, this is because when we await processing to complete, we need to return to the same thread we began processing on.

Below is a basic example of its usage.

```

public static void Main(string[] args)
{
        //Alter any configuration properties required.         
        MMALCameraConfig.EnableAnnotate = true;
        MMALCameraConfig.Annotate = new AnnotateImage { ShowDateText = true, ShowTimeText = true };
		
        using (MMALCamera cam = MMALCamera.Instance)
		{
			//Create our component pipeline. 
			cam.AddEncoder(new MMALImageEncoder(), cam.Camera.StillPort)
			   .CreatePreviewComponent(new MMALNullSinkComponent())
			   .ConfigureCamera();
			
			AsyncContext.Run(async () =>
			{
				//Take a picture on output port '0' of the encoder connected to the Camera's still port, sending the output to a filestream.
				using (var fs = File.Create("/home/pi/test.jpg"))
				{
					await cam.TakePicture(cam.Camera.StillPort, 0, new StreamCaptureResult(fs));
				}                                        
			});   
		}
}

```

##Status

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



##License

MIT license 

Copyright (c) 2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

##Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.
