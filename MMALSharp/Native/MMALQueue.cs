using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALQueue
    {
        //MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_create", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_QUEUE_T* mmal_queue_create();

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_put", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_queue_put(MMAL_QUEUE_T* ptr, MMAL_QUEUE_T* ptr2);

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_put_back", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_queue_put_back(MMAL_QUEUE_T* ptr, MMAL_BUFFER_HEADER_T* header);

        //MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_get", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_BUFFER_HEADER_T* mmal_queue_get(MMAL_QUEUE_T* ptr);

        //MMAL_QUEUE_T*
        [DllImport("libmmal.so", EntryPoint = "mmal_queue_wait", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_BUFFER_HEADER_T* mmal_queue_wait(MMAL_QUEUE_T* ptr);

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_length", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint mmal_queue_length(MMAL_QUEUE_T* ptr);

        [DllImport("libmmal.so", EntryPoint = "mmal_queue_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_queue_destroy(MMAL_QUEUE_T* ptr);
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
