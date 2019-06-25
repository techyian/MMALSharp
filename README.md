# MMALSharp - C#/.NET API for the Raspberry Pi camera 

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp) 
[![Join the chat at https://gitter.im/MMALSharp/Lobby](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MMALSharp/Lobby?utm_source=share-link&utm_medium=link&utm_campaign=share-link)

**If you like this project, please support it by giving it a** 
<a class="github-button" href="https://github.com/techyian/MMALSharp" data-icon="octicon-star" data-show-count="true" aria-label="Star techyian/MMALSharp on GitHub">Star</a>

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

## Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.

### Contributors

**Daniel Lerch** [@daniel-lerch](https://github.com/daniel-lerch)
