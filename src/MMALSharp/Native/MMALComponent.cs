using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALComponent
    {
        //name: char* * comp: MMAL_COMPONENT_T**    
        [DllImport("libmmal.so", EntryPoint = "mmal_component_create", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_component_create(string name, IntPtr* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_acquire", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_component_acquire(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_release", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_component_release(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_component_destroy(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_enable", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_component_enable(MMAL_COMPONENT_T* comp);

        [DllImport("libmmal.so", EntryPoint = "mmal_component_disable", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_component_disable(MMAL_COMPONENT_T* comp);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_COMPONENT_PRIVATE_T
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_COMPONENT_T
    {
        private IntPtr priv, userData;
        private char* name;
        private uint isEnabled;
        private MMAL_PORT_T* control;
        private uint inputNum;
        private MMAL_PORT_T** input;
        private uint outputNum;
        private MMAL_PORT_T** output;
        private uint clockNum;
        private MMAL_PORT_T** clock;
        private uint portNum;
        private MMAL_PORT_T** port;
        private uint id;

        public IntPtr Priv => priv;
        public IntPtr UserData => userData;
        public char* Name => name;
        public uint IsEnabled => isEnabled;
        public MMAL_PORT_T* Control => control;
        public uint InputNum => inputNum;
        public MMAL_PORT_T** Input => input;
        public uint OutputNum => outputNum;
        public MMAL_PORT_T** Output => output;
        public uint ClockNum => clockNum;
        public MMAL_PORT_T** Clock => clock;
        public uint PortNum => portNum;
        public MMAL_PORT_T** Port => port;
        public uint Id => id;

        public MMAL_COMPONENT_T(IntPtr priv, IntPtr userData, char* name, uint isEnabled, MMAL_PORT_T* control, uint inputNum,
                                MMAL_PORT_T** input, uint outputNum, MMAL_PORT_T** output, uint clockNum, MMAL_PORT_T** clock,
                                uint portNum, MMAL_PORT_T** port, uint id)
        {
            this.priv = priv;
            this.userData = userData;
            this.name = name;
            this.isEnabled = isEnabled;
            this.control = control;
            this.inputNum = inputNum;
            this.input = input;
            this.outputNum = outputNum;
            this.output = output;
            this.clockNum = clockNum;
            this.clock = clock;
            this.portNum = portNum;
            this.port = port;
            this.id = id;            
        }
    }





}
