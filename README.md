# MMALSharp - C# wrapper to Broadcom's MMAL and API to the Raspberry Pi camera 

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp) 
[![Join the chat at https://gitter.im/MMALSharp/Lobby](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MMALSharp/Lobby?utm_source=share-link&utm_medium=link&utm_campaign=share-link)

**If you like this project, please support it by giving it a star!** 
![GitHub stars](https://img.shields.io/github/stars/techyian/MMALSharp.svg?style=popout)

MMALSharp is a C# wrapper around the MMAL library designed by Broadcom. It exposes many elements of MMAL and in addition provides an easy to use, asynchronous API to the Raspberry Pi Camera Module.

MMALSharp supports the following runtimes:

1. Mono 4.x 
2. .NET Standard 2.0

## Installation

MMALSharp NuGet package:
[![NuGet version](https://badge.fury.io/nu/MMALSharp.svg)](https://badge.fury.io/nu/MMALSharp)

```
PM> Install-Package MMALSharp
```

MMALSharp.FFmpeg NuGet package:
[![NuGet version](https://badge.fury.io/nu/MMALSharp.FFmpeg.svg)](https://badge.fury.io/nu/MMALSharp.FFmpeg)

```
PM> Install-Package MMALSharp.FFmpeg
```

Pre-release builds can be found on [MyGet](https://www.myget.org/gallery/mmalsharp):

## Basic Examples

Take a JPEG image using YUV420 encoding:

```csharp

public void TakePicture()
{
    // Singleton initialized lazily. Reference once in your application.
    MMALCamera cam = MMALCamera.Instance;

    using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))        
    {            
        await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
    }
    
    // Cleanup disposes all unmanaged resources and unloads Broadcom library. To be called when no more processing is to be done
    // on the camera.
    cam.Cleanup();
}

```

Take a H.264 video using YUV420 encoding at 30 fps:

```csharp

public void TakeVideo()
{
    // Singleton initialized lazily. Reference once in your application.
    MMALCamera cam = MMALCamera.Instance;

    using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/", "avi"))        
    {    
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
                
        await cam.TakeVideo(vidCaptureHandler, cts.Token);
    }   

    // Cleanup disposes all unmanaged resources and unloads Broadcom library. To be called when no more processing is to be done
    // on the camera.
    cam.Cleanup();
}

```


## Documentation

For full installation instructions for Mono 4.x and .NET Core, including configuration and examples - please visit the [Wiki](https://github.com/techyian/MMALSharp/wiki) site.


## License

MIT license 

Copyright (c) 2016-2019 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

## Contributors

I want to say a big thank you to those of you who have helped develop MMALSharp over the years, your contributions are most appreciated. In addition, I'd like to say thanks to Dave Jones [@waveform80](https://github.com/waveform80) for your work on picamera which gave me the inspiration to start this project.