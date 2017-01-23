# MMALSharp

MMALSharp is an unofficial C# API for the Raspberry Pi camera. It is currently an early experimental build which features the ability to 
take pictures with your Raspberry Pi.

##Basic Usage

Using the library is relatively simple. Initially, you are required to create an instance of the `MMALCameraConfig` class, changing any 
properties within it as you see it (default values are set automatically on your behalf). Next, you should create an instance of the
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
            cam.ConfigureCamera().TakePicture("/home/pi/test2.jpg");
            Console.WriteLine("Brightness " + cam.Brightness);
            Console.WriteLine("Contrast " + cam.Contrast);
            Console.WriteLine("Sharpness " + cam.Sharpness);            
        }                          
}

```

##Building

The project is currently targeting .NET framework 4.5.2, and therefore requires Mono 4.x.

##Status

As mentioned previously, this is currently an experimental build and therefore a lot of functionality is not fully tested, however
a number of common features are working correctly.

This library has only been tested on a Raspberry Pi 1 Model B so far with the NoIR v1 camera module, it may or may not work with
later Pi models and/or camera modules. 

##Known Issues

Following taking a picture, there is an issue currently with the JPEG encoder whereby it does not send the final buffer frame. Callbacks
from unmanaged-managed code usually happens on a worker background thread, however, when disposing of the MMAL component classes, the final
buffer frame is dumped onto the main foreground thread which prevents the application from closing cleanly. I need to do some more
investigation as to why this happens.

##License

MIT license 

Copyright (c) 2010-2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

##Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python buffer header conversions have saved me numerous hours so far. 
Thank you very much.
