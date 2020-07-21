// <copyright file="MMALQueue.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALQueue
    {
#pragma warning disable IDE1006 // Naming Styles
        // MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_QUEUE_T* mmal_queue_create();

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_put", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_queue_put(MMAL_QUEUE_T* ptr, MMAL_BUFFER_HEADER_T* header);

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_put_back", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_queue_put_back(MMAL_QUEUE_T* ptr, MMAL_BUFFER_HEADER_T* header);

        // MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_BUFFER_HEADER_T* mmal_queue_get(MMAL_QUEUE_T* ptr);

        // MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_wait", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_BUFFER_HEADER_T* mmal_queue_wait(MMAL_QUEUE_T* ptr);

        // MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_timedwait", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_BUFFER_HEADER_T* mmal_queue_timedwait(MMAL_QUEUE_T* ptr, int waitms);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_length", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe uint mmal_queue_length(MMAL_QUEUE_T* ptr);

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_queue_destroy(MMAL_QUEUE_T* ptr);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_QUEUE_T
    {
        private uint length;
        private MMAL_BUFFER_HEADER_T* first;
        private MMAL_BUFFER_HEADER_T** last;

        public uint Length => length;
        public MMAL_BUFFER_HEADER_T* First => first;
        public MMAL_BUFFER_HEADER_T** Last => last;

        public MMAL_QUEUE_T(uint length, MMAL_BUFFER_HEADER_T* first, MMAL_BUFFER_HEADER_T** last)
        {
            this.length = length;
            this.first = first;
            this.last = last;
        }
    }
}
