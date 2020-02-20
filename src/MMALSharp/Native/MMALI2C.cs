using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line
    public class MMALI2C
    {
        [DllImport("libc.so.6", EntryPoint = "open", SetLastError = true)]
        internal static extern int Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int Ioctl(int fd, int request, IntPtr data);

        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int IoctlByte(int fd, int request, byte data);

        [DllImport("libc.so.6", EntryPoint = "read", SetLastError = true)]
        internal static extern int Read(int handle, IntPtr data, int length);

        [DllImport("libc.so.6", EntryPoint = "write", SetLastError = true)]
        internal static extern int Write(int handle, IntPtr data, int length);

        [DllImport("libc.so.6", EntryPoint = "close", SetLastError = true)]
        internal static extern int Close(int handle);

        [StructLayout(LayoutKind.Sequential)]
        public struct I2CMsg
        {
            private ushort _addr, _flags, _len;
            private IntPtr _buf;
          
            public ushort Addr => _addr;
            public ushort Flags => _flags;
            public ushort Len => _len;
            public IntPtr Buf => _buf;

            public I2CMsg(byte addr, ushort flags, ushort len, IntPtr buf)
            {
                _addr = addr;
                _flags = flags;
                _len = len;
                _buf = buf;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct I2CRdwrIoctlData
        {
            private IntPtr _msgs;
            private int _nMsgs;

            public IntPtr Msgs => _msgs;
            public int NMsgs => _nMsgs;

            public I2CRdwrIoctlData(IntPtr msgs, int nMsgs)
            {
                _msgs = msgs;
                _nMsgs = nMsgs;
            }
        }
    }
}
