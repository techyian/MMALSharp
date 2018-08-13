using System;
using System.IO;
using System.Text;

namespace MMALSharp.Processors
{
    public class BayerHandler : IFrameProcessor
    {
        public CameraVersion CameraVersion { get; }
        public Stream WorkingStream { get; }
        
        private const int BayerMetaLengthV1 = 6404096;
        private const int BayerMetaLengthV2 = 10270208;
        
        public BayerHandler(CameraVersion camVersion, Stream stream)
        {
            this.CameraVersion = camVersion;
            this.WorkingStream = stream;
        }

        public void Apply(byte[] store)
        {
            byte[] array = null;
            
            switch (this.CameraVersion)
            {
                case CameraVersion.OV5647:
                    array = new byte[BayerMetaLengthV1];
                    Array.Copy(store, store.Length - BayerMetaLengthV1, array, 0, BayerMetaLengthV1);
                    break;
                case CameraVersion.IMX219:
                    array = new byte[BayerMetaLengthV2];
                    Array.Copy(store, store.Length - BayerMetaLengthV2, array, 0, BayerMetaLengthV2);
                    break;
            }

            byte[] meta = new byte[4];
            Array.Copy(array, 0, meta, 0, 4);

            if (Encoding.ASCII.GetString(meta) != "BRCM")
            {
                throw new Exception("Could not find Bayer metadata in header");
            }

            this.WorkingStream.Write(array, 0, array.Length);
        }
    }
}
