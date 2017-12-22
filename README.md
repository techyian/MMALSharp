# MMALSharp

[![Build status](https://ci.appveyor.com/api/projects/status/r3o4bqxektnulw7l?svg=true)](https://ci.appveyor.com/project/techyian/mmalsharp)

MMALSharp is an unofficial C# API for the Raspberry Pi camera. Under the hood, MMALSharp makes use of the native MMAL interface designed by Broadcom.

The project is in early stages of development, however progress is good, please see the status of each component below:

- [x] Camera
- [x] Camera Info
- [x] Renderers (Null sink & Video)
- [x] Resizer
- [x] Splitter
- [x] Image Encoder
- [ ] Image Decoder
- [x] Video Encoder
- [ ] Video Decoder

** Please clone from Master branch if building from source. Dev branch not guaranteed to be stable. **

MMALSharp supports the following runtimes:

1. Mono 4.x 
2. .NET Standard 2.0

## Documentation

For full installation instructions for Mono 4.x and .NET Core, including configuration and examples - please visit the [Wiki](https://github.com/techyian/MMALSharp/wiki) site.

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

## License

MIT license 

Copyright (c) 2017 Ian Auty

Raspberry Pi is a trademark of the Raspberry Pi Foundation

## Special thanks

Dave Jones [@waveform80](https://github.com/waveform80) - your Python header conversions have saved me numerous hours so far. 
Thank you very much.
