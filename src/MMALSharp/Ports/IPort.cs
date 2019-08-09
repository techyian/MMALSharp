
using System;
using System.Drawing;
using System.Threading.Tasks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    public interface IPort : IMMALObject
    {
        unsafe MMAL_PORT_T* Ptr { get; }
        PortType PortType { get; }
        IComponent ComponentReference { get; }
        IConnection ConnectedReference { get; }
        IBufferPool BufferPool { get; }

        Guid Guid { get; }
        MMALEncoding EncodingType { get; }
        MMALEncoding PixelFormat { get; }
        ICaptureHandler Handler { get; } // --not sure about this being here? What about more specific types??

        MMALPortConfig PortConfig { get; }
        string Name { get; }
        bool Enabled { get; }
        int BufferNumMin { get; }
        int BufferSizeMin { get; }
        int BufferAlignmentMin { get; }
        int BufferNumRecommended { get; }
        int BufferSizeRecommended { get; }
        int BufferNum { get; }
        int BufferSize { get; }
        MMAL_ES_FORMAT_T Format { get; }
        Resolution Resolution { get; }
        Rectangle Crop { get; }
        MMAL_RATIONAL_T FrameRate { get; }
        MMALEncoding VideoColorSpace { get; }
        int CropWidth { get; }
        int CropHeight { get; }
        int NativeEncodingType { get; }
        int NativeEncodingSubformat { get; }
        int Bitrate { get; }
        MMAL_RATIONAL_T Par { get; }
        bool ZeroCopy { get; set; }
        TaskCompletionSource<bool> Trigger { get; }
        void EnablePort(IntPtr callback);

        void DisablePort();

        void Commit();

        void ShallowCopy(IPort destination);

        void ShallowCopy(IBufferEvent eventFormatSource);

        void FullCopy(IPort destination);

        void FullCopy(IBufferEvent eventFormatSource);

        void Flush();

        void SendBuffer(IBuffer buffer);

        void SendAllBuffers(bool sendBuffers = true);

        void SendAllBuffers(IBufferPool pool);

        void DestroyPortPool();

        void InitialiseBufferPool();

        void ExtraDataAlloc(int size);
    }
}
