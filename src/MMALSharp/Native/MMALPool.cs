// <copyright file="MMALPool.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALPool
    {
        public delegate void mmal_pool_allocator_alloc_t(IntPtr ptr, uint value);
        public delegate void mmal_pool_allocator_free_t(IntPtr ptr, IntPtr ptr2);

        // typedef - Pointer to MMAL_POOL_T struct * Pointer to MMAL_BUFFER_HEADER_T struct * Pointer to void -> Returns MMAL_BOOL_T struct
        public unsafe delegate int MMAL_POOL_BH_CB_T(MMAL_POOL_T* pool, MMAL_BUFFER_HEADER_T* buffer);

#pragma warning disable IDE1006 // Naming Styles

        // MMAL_POOL_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_pool_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mmal_pool_create(uint bufferNum, uint bufferSize);

        // MMAL_POOL_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_pool_create_with_allocator", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mmal_pool_create_with_allocator(uint headers,
                                                            uint payload_size,
                                                            IntPtr allocator_context,
                                                            mmal_pool_allocator_alloc_t allocator_alloc,
                                                            mmal_pool_allocator_free_t allocator_free);

        [DllImport("libmmal.so", EntryPoint = "mmal_pool_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_pool_destroy(MMAL_POOL_T* pool);

        [DllImport("libmmal.so", EntryPoint = "mmal_pool_resize", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_pool_resize(MMAL_POOL_T* pool, uint headers, uint payload_size);
                
        [DllImport("libmmal.so", EntryPoint = "mmal_pool_callback_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_pool_callback_set(MMAL_POOL_T* pool, [MarshalAs(UnmanagedType.FunctionPtr)] MMAL_POOL_BH_CB_T cb, IntPtr userdata);

        [DllImport("libmmal.so", EntryPoint = "mmal_pool_pre_release_callback_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_pool_pre_release_callback_set(MMAL_POOL_T* pool, [MarshalAs(UnmanagedType.FunctionPtr)] MMAL_POOL_BH_CB_T cb, IntPtr userdata);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_POOL_T
    {
        private MMAL_QUEUE_T* queue;
        private uint headersNum;
        private IntPtr header;

        public MMAL_QUEUE_T* Queue => queue;
        public uint HeadersNum => headersNum;
        public IntPtr Header => header;

        public MMAL_POOL_T(MMAL_QUEUE_T* queue, uint headersNum, IntPtr header)
        {
            this.queue = queue;
            this.headersNum = headersNum;
            this.header = header;
        }
    }
}
