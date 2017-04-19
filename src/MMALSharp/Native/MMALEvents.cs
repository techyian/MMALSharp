using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALEvents
    {
        public static int MMAL_EVENT_ERROR = MMALUtil.MMAL_FOURCC("ERRO");
        public static int MMAL_EVENT_EOS = MMALUtil.MMAL_FOURCC("EEOS");
        public static int MMAL_EVENT_FORMAT_CHANGED = MMALUtil.MMAL_FOURCC("EFCH");
        public static int MMAL_EVENT_PARAMETER_CHANGED = MMALUtil.MMAL_FOURCC("EPCH");
                
        [DllImport("libmmal.so", EntryPoint = "mmal_event_format_changed_get", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_EVENT_FORMAT_CHANGED_T* mmal_event_format_changed_get(MMAL_BUFFER_HEADER_T* buffer);

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_EVENT_END_OF_STREAM_T
    {
        private MMALPort.MMAL_PORT_TYPE_T portType;
        private uint portIndex;

        public MMALPort.MMAL_PORT_TYPE_T PortType => portType;
        public uint PortIndex => portIndex;

        public MMAL_EVENT_END_OF_STREAM_T(MMALPort.MMAL_PORT_TYPE_T portType, uint portIndex)
        {
            this.portType = portType;
            this.portIndex = portIndex;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_EVENT_FORMAT_CHANGED_T
    {
        private uint bufferSizeMin, bufferNumMin, bufferSizeRecommended, bufferNumRecommended;
        private MMAL_ES_FORMAT_T* format;

        public uint BufferSizeMin => bufferSizeMin;
        public uint BufferNumMin => bufferNumMin;
        public uint BufferSizeRecommended => bufferSizeRecommended;
        public uint BufferNumRecommended => bufferNumRecommended;
        public MMAL_ES_FORMAT_T* Format => format;

        public MMAL_EVENT_FORMAT_CHANGED_T(uint bufferSizeMin, uint bufferNumMin, uint bufferSizeRecommended, uint bufferNumRecommended,
                                           MMAL_ES_FORMAT_T* format)
        {
            this.bufferSizeMin = bufferSizeMin;
            this.bufferNumMin = bufferNumMin;
            this.bufferSizeRecommended = bufferSizeRecommended;
            this.bufferNumRecommended = bufferNumRecommended;
            this.format = format;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_EVENT_PARAMETER_CHANGED_T
    {
        public MMAL_PARAMETER_HEADER_T hdr;

        public MMAL_PARAMETER_HEADER_T Hdr => hdr;

        public MMAL_EVENT_PARAMETER_CHANGED_T(MMAL_PARAMETER_HEADER_T hdr)
        {
            this.hdr = hdr;
        }
    }



}
