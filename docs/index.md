# MMALSharp

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp)

MMALSharp is an unofficial C# API for the Raspberry Pi camera. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

The project is in early stages of development, however the ability to take pictures and record video is working as expected (H.264 & MJPEG).

MMALSharp supports the following runtimes:

1. Mono 4.x 
2. .NET Core 2.0 (beta) with .NET Standard 1.6.


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
