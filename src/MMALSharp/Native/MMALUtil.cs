using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALUtil
    {
        public static int VCOS_ALIGN_UP(int value, int roundTo)
        {
            return (int)(Math.Ceiling(value / Convert.ToDouble(roundTo)) * roundTo);
        }

        public static int MMAL_FOURCC(string s)
        {
            int a1 = s[0];
            int b1 = s[1];
            int c1 = s[2];
            int d1 = s[3];
            return ((a1) | (b1 << 8) | (c1 << 16) | (d1 << 24));
        }

        public enum MMAL_STATUS_T
        {
            MMAL_SUCCESS,
            MMAL_ENOMEM,
            MMAL_ENOSPC,
            MMAL_EINVAL,
            MMAL_ENOSYS,
            MMAL_ENOENT,
            MMAL_ENXIO,
            MMAL_EIO,
            MMAL_ESPIPE,
            MMAL_ECORRUPT,
            MMAL_ENOTREADY,
            MMAL_ECONFIG,
            MMAL_EISCONN,
            MMAL_ENOTCONN,
            MMAL_EAGAIN,
            MMAL_EFAULT,
            MMAL_STATUS_MAX = 0x7FFFFFFF
        }
               
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_boolean", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_boolean(MMAL_PORT_T* port, uint id, int value);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_boolean", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_boolean(MMAL_PORT_T* port, uint id, ref int value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_uint64", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_uint64(MMAL_PORT_T* port, uint id, ulong value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_uint64", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_uint64(MMAL_PORT_T* port, uint id, ref ulong value);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_int64", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_int64(MMAL_PORT_T* port, uint id, long value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_int64", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_int64(MMAL_PORT_T* port, uint id, ref long value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_uint32", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_uint32(MMAL_PORT_T* port, uint id, uint value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_uint32", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_uint32(MMAL_PORT_T* port, uint id, ref uint value);
                
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_int32", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_int32(MMAL_PORT_T* port, uint id, int value);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_int32", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_int32(MMAL_PORT_T* port, uint id, ref int value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_rational", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_rational(MMAL_PORT_T* port, uint id, MMAL_RATIONAL_T value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get_rational", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_get_rational(MMAL_PORT_T* port, uint id, ref MMAL_RATIONAL_T value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_string", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_string(MMAL_PORT_T* port, uint id, [MarshalAs(UnmanagedType.LPTStr)] string value);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_port_parameter_set_bytes(MMAL_PORT_T* port, uint id, byte* data, uint size);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_util_port_set_uri", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_util_port_set_uri(MMAL_PORT_T* port, [MarshalAs(UnmanagedType.LPTStr)] string uri);

        [DllImport("libmmal.so", EntryPoint = "mmal_util_set_display_region", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_util_set_display_region(MMAL_PORT_T* port, MMAL_DISPLAYREGION_T* region);

        [DllImport("libmmal.so", EntryPoint = "mmal_util_camera_use_stc_timestamp", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_util_camera_use_stc_timestamp(MMAL_PORT_T* port, MMAL_CAMERA_STC_MODE_T mode);

        [DllImport("libmmal.so", EntryPoint = "mmal_util_get_core_port_stats", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_STATUS_T mmal_util_get_core_port_stats(MMAL_PORT_T* port, MMAL_CORE_STATS_DIR dir, int reset, ref MMAL_CORE_STATISTICS_T stats);

        [DllImport("libmmal.so", EntryPoint = "mmal_status_to_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern string mmal_status_to_string(MMAL_STATUS_T status);

        [DllImport("libmmal.so", EntryPoint = "mmal_encoding_stride_to_width", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint mmal_encoding_stride_to_width(uint encoding, uint stride);

        [DllImport("libmmal.so", EntryPoint = "mmal_encoding_width_to_stride", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint mmal_encoding_width_to_stride(uint encoding, uint width);

        [DllImport("libmmal.so", EntryPoint = "mmal_encoding_get_slice_variant", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint mmal_encoding_get_slice_variant(uint encoding);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_type_to_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern string mmal_port_type_to_string(MMALPort.MMAL_PORT_TYPE_T pType);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_alloc_get", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_PARAMETER_HEADER_T* mmal_port_parameter_alloc_get(MMAL_PORT_T* port, uint id, uint size, ref MMAL_STATUS_T status);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_free", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_port_parameter_free(MMAL_PARAMETER_HEADER_T* header);

        [DllImport("libmmal.so", EntryPoint = "mmal_buffer_header_copy_header", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_buffer_header_copy_header(MMAL_BUFFER_HEADER_T* dest, MMAL_BUFFER_HEADER_T* src);
                
        [DllImport("libmmal.so", EntryPoint = "mmal_port_pool_create", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_POOL_T* mmal_port_pool_create(MMAL_PORT_T* port, int headers, int payload_size);

        [DllImport("libmmal.so", EntryPoint = "mmal_port_pool_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_port_pool_destroy(MMAL_PORT_T* port, MMAL_POOL_T* pool);

        [DllImport("libmmal.so", EntryPoint = "mmal_log_dump_port", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_log_dump_port(MMAL_PORT_T* port);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_log_dump_format", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_log_dump_format(MMAL_ES_FORMAT_T* format);

        [DllImport("libmmal.so", EntryPoint = "mmal_util_get_port", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_PORT_T mmal_util_get_port(MMAL_COMPONENT_T* comp, MMALPort.MMAL_PORT_TYPE_T pType, uint index);
        
        [DllImport("libmmal.so", EntryPoint = "mmal_4cc_to_string", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern string mmal_4cc_to_string([MarshalAs(UnmanagedType.LPTStr)] string buffer, ushort len, uint fourcc);

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_RECT_T
    {
        private int x, y, width, height;

        public int X => x;
        public int Y => y;        
        public int Width => width;
        public int Height => height;

        public MMAL_RECT_T(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.width = width;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_FLOAT_RECT_T
    {
        private double x, y, width, height;

        public double X => x;
        public double Y => y;        
        public double Width => width;
        public double Height => height;

        public MMAL_FLOAT_RECT_T(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;            
            this.width = width;
            this.height = height;
        }
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_RATIONAL_T
    {
        private int num, den;

        public int Num => num;
        public int Den => den;

        public MMAL_RATIONAL_T(int num, int den)
        {
            this.num = num;
            this.den = den;
        }
    }
}
