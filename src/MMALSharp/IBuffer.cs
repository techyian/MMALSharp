// <copyright file="IBuffer.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Represents a buffer header.
    /// </summary>
    public interface IBuffer : IMMALObject
    {
        /// <summary>
        /// Defines what the buffer header contains. This is a FourCC with 0 as a special value meaning stream data.
        /// </summary>
        uint Cmd { get; }

        /// <summary>
        /// Allocated size in bytes of payload buffer.
        /// </summary>
        uint AllocSize { get; }

        /// <summary>
        /// Number of bytes currently used in the payload buffer (starting from offset).
        /// </summary>
        uint Length { get; }

        /// <summary>
        /// Offset in bytes to the start of valid data in the payload buffer.
        /// </summary>
        uint Offset { get; }

        /// <summary>
        /// Flags describing properties of a buffer header.
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// Presentation timestamp in microseconds.
        /// </summary>
        long Pts { get; }

        /// <summary>
        /// Decode timestamp in microseconds (dts = pts, except in the case of video streams with B frames).
        /// </summary>
        long Dts { get; }

        /// <summary>
        /// Accessor to the specific type this buffer header represents.
        /// </summary>
        MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type { get; }

        /// <summary>
        /// List of properties associated with this buffer header.
        /// </summary>
        List<MMALBufferProperties> Properties { get; }

        /// <summary>
        /// List of events associated with this buffer header.
        /// </summary>
        List<int> Events { get; }

        /// <summary>
        /// Native pointer that represents this buffer header.
        /// </summary>
        unsafe MMAL_BUFFER_HEADER_T* Ptr { get; }

        /// <summary>
        /// Print the properties associated with this buffer header to console.
        /// </summary>
        void PrintProperties();

        /// <summary>
        /// Writes events associated with the buffer header to log.
        /// </summary>
        void ParseEvents();

        /// <summary>
        /// Checks whether a buffer header contains a certain status code.
        /// </summary>
        /// <param name="property">The status code.</param>
        /// <returns>True if the buffer header contains the status code.</returns>
        bool AssertProperty(MMALBufferProperties property);

        /// <summary>
        /// Gathers all data in this payload and returns as a byte array.
        /// </summary>
        /// <returns>A byte array containing the image frame.</returns>
        byte[] GetBufferData();

        /// <summary>
        /// Writes user provided image data into a buffer header.
        /// </summary>
        /// <param name="source">The array of image data to write to buffer header.</param>
        /// <param name="length">The length of the data being written.</param>
        /// <param name="eof">Signal that we've reached the end of the input file.</param>
        void ReadIntoBuffer(byte[] source, int length, bool eof);

        /// <summary>
        /// Acquire a buffer header. Acquiring a buffer header increases a reference counter on it and makes 
        /// sure that the buffer header won't be recycled until all the references to it are gone.
        /// </summary>
        void Acquire();

        /// <summary>
        /// Release a buffer header. Releasing a buffer header will decrease its reference counter and when no more references are left, 
        /// the buffer header will be recycled by calling its 'release' callback function.
        /// </summary>
        void Release();

        /// <summary>
        /// Reset a buffer header. Resets all header variables to default values.
        /// </summary>
        void Reset();
    }
}
