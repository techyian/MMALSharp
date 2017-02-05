# MMALSharp

MMALSharp is an unofficial C# API for the Raspberry Pi camera. It is currently an early experimental build which features the ability to 
take pictures with your Raspberry Pi.

##Building

The project is currently targeting .NET framework 4.5.2, and therefore requires Mono 4.x.

MMALSharp uses FAKE for building, and Paket for dependency management. 

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
`MMALCamera` class - this class is the main object to work with in the library which grants access to the `TakePhoto` method.

```

public static void Main(string[] args)
{
        MMALCameraConfig config = new MMALCameraConfig
        {
            Sharpness = 100,            
            Contrast = 10,
            ImageEffect = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NEGATIVE			
        };

        using (MMALCamera cam = new MMALCamera(config))
        {
            cam.ConfigureCamera().TakePicture(new FileCaptureHandler("/home/pi/test2.jpg"), MMALEncodings.MMAL_ENCODING_JPEG, 90);
            Console.WriteLine(string.Format("Brightness: {0}", cam.Brightness));
            Console.WriteLine(string.Format("Contrast: {0}", cam.Contrast));
            Console.WriteLine(string.Format("Sharpness: {0}", cam.Sharpness));            
        }                          
}

```



##Status

This is currently an experimental build and therefore a lot of functionality is not fully tested, however
a number of common features are working correctly.

The library has currently been tested on the following Raspberry Pi devices:

* Raspberry Pi 1 Model B (512mb)
* Raspberry Pi Zero

Both the SUNNY and Sony camera modules are currently working as expected.

The encoding can be configured in the `MMALCameraConfig` class, i.e. 

```
MMALCameraConfig config = new MMALCameraConfig
{
    Encoding = MMALEncodings.MMAL_ENCODING_BMP            
};

using (MMALCamera cam = new MMALCamera(config))
{
    cam.ConfigureCamera().TakePicture(new FileCaptureHandler("/home/pi/test2.bmp"));              
}  

```


##License

MIT license 

Copyright (c) 2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

##Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.
