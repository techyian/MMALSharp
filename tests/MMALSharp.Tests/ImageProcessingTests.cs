// <copyright file="ImageProcessingTests.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Ports;
using Xunit;
using MMALSharp.Processors.Effects;
using System;
using System.Text;
using MMALSharp.Processors;
using System.IO;
using System.Drawing.Imaging;

namespace MMALSharp.Tests
{
    public class ImageProcessingTests : TestBase
    {
        public ImageProcessingTests(MMALFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task SharpenKernelProcessor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("SharpenKernelProcessor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imgCaptureHandler.Manipulate(context =>
                {
                    context.Apply(new SharpenProcessor());
                }, ImageFormat.Jpeg);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task EdgeDetectionKernelProcessor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("EdgeDetectionKernelProcessor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imgCaptureHandler.Manipulate(context =>
                {
                    context.Apply(new EdgeDetection(EDStrength.High));
                }, ImageFormat.Jpeg);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task GaussianBlurKernelProcessor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("GaussianBlurKernelProcessor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imgCaptureHandler.Manipulate(context =>
                {
                    context.Apply(new GaussianProcessor(GaussianMatrix.Matrix3x3));
                }, ImageFormat.Jpeg);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task StripBayerData()
        {
            TestHelper.BeginTest("Image - StripBayerData");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            string filepath = string.Empty;

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "raw"))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(true))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, quality: 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imgCaptureHandler.Manipulate(context =>
                {
                    context.StripBayerMetadata(CameraVersion.OV5647);
                }, ImageFormat.Jpeg);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                filepath = imgCaptureHandler.GetFilepath();
            }

            byte[] meta = new byte[4];

            var array = File.ReadAllBytes(filepath);

            // Uncomment depending on which version of the camera you're using.

            // Array.Copy(array, array.Length - BayerMetaProcessor.BayerMetaLengthV1, meta, 0, 4);
            Array.Copy(array, array.Length - BayerMetaProcessor.BayerMetaLengthV2, meta, 0, 4);

            Assert.True(Encoding.ASCII.GetString(meta) == "BRCM");
        }
    }
}
