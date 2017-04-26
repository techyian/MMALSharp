# Examples

## Image capture

### Simple JPEG capture

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)                    
				.CreatePreviewComponent(new MMALNullSinkComponent())
				.ConfigureCamera();
				
			await cam.TakePicture(cam.Camera.StillPort);
			
		}
	});		
}

```

### Timelapse mode

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)                    
				.CreatePreviewComponent(new MMALNullSinkComponent())
				.ConfigureCamera();
				
			//Take pictures every 5 seconds for 1 minute as a timelapse. 
            await cam.TakePictureTimelapse(cam.Camera.StillPort, new Timelapse { Mode = TimelapseMode.Second, Value = 5, Timeout = DateTime.Now.AddMinutes(1) });
			
		}
	}		
}

```


### Timeout mode

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)                    
				.CreatePreviewComponent(new MMALNullSinkComponent())
				.ConfigureCamera();
				
			//Take pictures continuously for 5 minutes
            await cam.TakePictureTimeout(cam.Camera.StillPort, DateTime.Now.AddMinutes(5));
			
		}
	});		
}

```


### Change encoding type


```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)                    
				.CreatePreviewComponent(new MMALNullSinkComponent())
				.ConfigureCamera();
				
			await cam.TakePicture(cam.Camera.StillPort);
			
		}
		
		//Exiting the using statement will clear up unmanaged resources used by the JPEG image encoder.
		
		imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "bmp");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, MMALEncoding.MMAL_ENCODING_BMP, MMALEncoding.MMAL_ENCODING_RGB32, 90))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)				
				.ConfigureCamera();
				
			await cam.TakePicture(cam.Camera.StillPort);
			
		}	
		
	});	
}

```



## Video recording

### Simple video recording

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos", "avi");
		
		using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, 10, 25))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(vidEncoder, cam.Camera.VideoPort)                    
				.CreatePreviewComponent(new MMALVideoRenderer())
				.ConfigureCamera();
				
			//Record video for 1 minute
            await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1));
			
		}
	});		
}

```

### Segmented recording mode

```

static void Main(string[] args)
{
	//Required for segmented recording mode
	MMALCameraConfig.InlineHeaders = true;

    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos", "avi");
		
		using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, 10, 25))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(vidEncoder, cam.Camera.VideoPort)                    
				.CreatePreviewComponent(new MMALVideoRenderer())
				.ConfigureCamera();
				
			//Record video for 1 minute, using segmented video record to split into multiple files every 30 seconds.
            await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1), new Split { Mode = TimelapseMode.Second, Value = 30 });
			
		}
	});		
}

```


### Change encoding type

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos", "avi");
		
		using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, 10, 25))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(vidEncoder, cam.Camera.VideoPort)                    
				.CreatePreviewComponent(new MMALVideoRenderer())
				.ConfigureCamera();
				
			//Record video for 1 minute
            await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1));
			
		}
		
		//Cleanup any resources used by the old encoder, and attach a new MJPEG encoder using I420 pixel format at 25mb/s bitrate.
		
		using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, MMALEncoding.MMAL_ENCODING_MJPEG, MMALEncoding.MMAL_ENCODING_I420, 25000000, 25, 90))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(vidEncoder, cam.Camera.VideoPort)                    
				.CreatePreviewComponent(new MMALVideoRenderer())
				.ConfigureCamera();
				
			//Record video for 1 minute
            await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1));
			
		}
				
	});		
}

```


## FFmpeg specific

For FFmpeg methods, you will need to install the latest version of FFmpeg from source - do not install from the Raspbian repositories as they don't have H.264 support.

### RTMP streaming

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		//An RTMP server needs to be listening on the address specified in the capture handler. I have used the Nginx RTMP module for testing.
		
		var ffmpegCaptureHandler = FFmpegCaptureHandler.RTMPStreamer("mystream", "rtmp://192.168.1.91:6767/live");

		//This will use the H.264 encoding type with I420 pixel format by default. Framerate is set at 15 fps, using a quality of 40 (lowest)
		
		using (var vidEncoder = new MMALVideoEncoder(ffmpegCaptureHandler, 40, 15))
		{
			cam.AddEncoder(vidEncoder, cam.Camera.VideoPort)                     
			   .CreatePreviewComponent(new MMALVideoRenderer())
			   .ConfigureCamera();

			/*
			 * Stream video for 5 minutes via RTMP using the *FFmpegCaptureHandler* class. 
			 * Note: FFmpeg must be installed for this method to work correctly and an appropriate RTMP server running such as https://github.com/arut/nginx-rtmp-module
			*/
			await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(5));
		}
	});		
}

```


### Raw video convert

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var ffmpegCaptureHandler = FFmpegCaptureHandler.RawVideoConvert("/home/pi/videos", ".avi");

		using (var vidEncoder = new MMALVideoEncoder(ffmpegCaptureHandler, 40, 15))
		{
			cam.AddEncoder(vidEncoder, cam.Camera.VideoPort)                     
			   .CreatePreviewComponent(new MMALVideoRenderer())
			   .ConfigureCamera();
			
			await cam.TakeVideo(cam.Camera.VideoPort, DateTime.Now.AddMinutes(1));
		}
	});		
}

```


### Images to video

```

static void Main(string[] args)
{
    MMALCamera cam = MMALCamera.Instance;
                                    
	AsyncContext.Run(async () =>
	{
		var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
		
		using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
		{
			//Create our component pipeline.         
			cam
				.AddEncoder(imgEncoder, cam.Camera.StillPort)                    
				.CreatePreviewComponent(new MMALNullSinkComponent())
				.ConfigureCamera();
				
			//Take pictures every 5 seconds for 1 minute as a timelapse. 
            await cam.TakePictureTimelapse(cam.Camera.StillPort, new Timelapse { Mode = TimelapseMode.Second, Value = 5, Timeout = DateTime.Now.AddMinutes(1) });
			
			//Processes the list of images you've taken with the *ImageStreamCaptureHandler* class into a video at 2fps.
            imgCaptureHandler.ImagesToVideo("/home/pi/videos", 2);
			
		}
	});		
}

```
