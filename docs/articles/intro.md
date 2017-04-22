# MMALSharp - Unofficial C# API to the Raspberry Pi Camera Module

## Introduction

Hi and welcome to MMALSharp. This project aims to provide a C# interface to the Raspberry Pi Camera Module, in an easy to use and extensible way.

MMAL (Multimedia Abstraction Layer) is a C library designed by Broadcom for use with the Videocore IV GPU found on the Raspberry Pi. Providing an abstraction layer upon another C library "OpenMAX",
MMAL exposes an API allowing developers to take images and record video from their Raspberry Pi which is easier to understand and consume.

The MMALSharp project brings you the functionality provided by the native MMAL library by using the C# Interop suite, this allows code in a Managed 
environment to call Native functions in C/C++.

## MMAL

MMAL introduces the concept of "Components" - resources which are responsible for processing data. Examples of Components include the Camera Module itself (see: `MMALCameraComponent`), Image and Video Encoders/Decoders (see: `MMALEncoderBase` inheritors) and Video renderers (a renderer is responsible for preview and overlay output to the Pi's display - see `MMALRendererBase` inheritors).

Components can be connected together in order to construct a pipeline by using "Ports" - these allow data to be transported from one component to another. 

Components feature a mixture of input/output ports, and in MMALSharp, a Component which has data passed to it is known as a "Downstream Component" (see: `MMALDownstramComponent` inheritors). In order to connect
two Components together, an output port is connected to the input port of a Downstream Component. Generally, the `MMALCameraComponent` would be the first Component in your pipeline which exposes 3 output ports: Still image, Video and
Preview ports. Following this, a user could connect an Image or Video Encoder which allows you to convert the raw image data into an encoded format such as JPEG or H.264.







