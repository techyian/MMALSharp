using System;
using System.Text;
using MMALSharp.Common;

namespace MMALSharp.Processors
{
    public class BayerMetaProcessor : IFrameProcessor
    {
        public CameraVersion CameraVersion { get; }
        
        private const int BayerMetaLengthV1 = 6404096;
        private const int BayerMetaLengthV2 = 10270208;
        
        public BayerMetaProcessor(CameraVersion camVersion)
        {
            this.CameraVersion = camVersion;
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

            store = new byte[array.Length];
            Array.Copy(array, store, array.Length);
        }
    }
}
