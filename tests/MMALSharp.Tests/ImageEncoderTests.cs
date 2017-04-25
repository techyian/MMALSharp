using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class ImageEncoderTests
    {
        MMALFixture fixture;

        public ImageEncoderTests(MMALFixture fixture)
        {
            this.fixture = fixture;
        }

        public static IEnumerable<object[]> TakePictureData
        {
            get
            {
                return new[]
                {
                    TestData.JPEGEncoderData,
                    TestData.GIFEncoderData,
                    TestData.PPMEncoderData,
                    TestData.PNGEncoderData,
                    TestData.TGAEncoderData,
                    TestData.BMPEncoderData                    
                };
            }
        }
                
        [Theory, MemberData(nameof(TakePictureData))]        
        public void TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", extension);
                
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, encodingType, pixelFormat, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                       .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                       .CreatePreviewComponent(new MMALNullSinkComponent())
                       .ConfigureCamera();

                    await fixture.MMALCamera.TakePicture(fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }

                Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");                
            });
        }

    }
}
