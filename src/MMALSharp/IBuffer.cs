using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp
{
    public interface IBuffer : IMMALObject
    {
        uint Cmd { get; }
        uint AllocSize { get; }
        uint Length { get; }
        uint Offset { get; }
        uint Flags { get; }
        long Pts { get; }
        long Dts { get; }
        MMAL_BUFFER_HEADER_TYPE_SPECIFIC_T Type { get; }
        List<MMALBufferProperties> Properties { get; }
        List<int> Events { get; }
        unsafe MMAL_BUFFER_HEADER_T* Ptr { get; }

        void PrintProperties();

        void ParseEvents();

        bool AssertProperty(MMALBufferProperties property);

        byte[] GetBufferData();

        void ReadIntoBuffer(byte[] source, int length, bool eof);

        void Acquire();

        void Release();

        void Reset();
    }
}
