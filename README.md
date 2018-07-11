# MMALSharp - C#/.NET API for the Raspberry Pi camera

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp)

MMALSharp is an unofficial C# API for the Raspberry Pi camera. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

MMALSharp supports the following runtimes:

1. Mono 4.x 
2. .NET Standard 2.0

## Installation

MMALSharp NuGet package:
[![NuGet version](https://badge.fury.io/nu/MMALSharp.svg)](https://badge.fury.io/nu/MMALSharp)

```
PM> Install-Package MMALSharp
```

## Basic Examples

Take a JPEG image using YUV420 encoding:

```csharp
static void Main(string[] args)
{                        
    MMALCamera cam = MMALCamera.Instance;

    AsyncContext.Run(async () =>
    {                
        using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))        
        {            
            await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
        }
    });

    cam.Cleanup();
}
```

Take a H.264 video using YUV420 encoding at 30 fps:

```csharp
static void Main(string[] args)
{                        
    MMALCamera cam = MMALCamera.Instance;

    AsyncContext.Run(async () =>
    {                
        using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/", "avi"))        
        {    
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            //Take video for 3 minutes.
            await cam.TakeVideo(vidCaptureHandler, cts.Token);
        }
    });

    cam.Cleanup();
}
```


## Documentation

For full installation instructions for Mono 4.x and .NET Core, including configuration and examples - please visit the [Wiki](https://github.com/techyian/MMALSharp/wiki) site.


## Notes & Known issues

When using more resource intensive encoders such as MMAL_ENCODING_BMP and the Sony IMX219 module, I've found it necessary to increase the memory split
to around 200mb or otherwise you'll receive an ENOSPC error due to insufficient resources.

Video decoder issue - I've tested a working pipeline as follows:

H264 YUV420 encode -> YUV420 decode -> MJPEG YUV420 encode does work, however the bitrate appears to be extremely low and the resulting video is very pixelated. I'm going to do 
some more investigating and see whether it's an issue with MMALSharp specifically or the native framework.

## License

MIT license 

Copyright (c) 2016-2018 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

## Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.

### Contributors

**Daniel Lerch** [@daniel-lerch](https://github.com/daniel-lerch)
