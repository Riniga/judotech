using AForge.Video;
using AForge.Video.DirectShow;

namespace Judoka.VideoStreamCapture
{
    public class VideoCapture
    {
        VideoCaptureDevice videoCaptureDevice;

        public bool Capturing = false;
        public FilterInfoCollection VideoInputDevice => new FilterInfoCollection(FilterCategory.VideoInputDevice);
        public VideoCapabilities[] SnapshotCapabilities => videoCaptureDevice.SnapshotCapabilities;



        public VideoCapture()
        {
            videoCaptureDevice = new VideoCaptureDevice();
        }

        internal void SetCaptureDevice(int index)
        {
            videoCaptureDevice = new VideoCaptureDevice(VideoInputDevice[index].MonikerString);
        }

        internal void Start(NewFrameEventHandler videoCaptureDevice_NewFrame)
        {
            videoCaptureDevice.Start();
            videoCaptureDevice.NewFrame += videoCaptureDevice_NewFrame;
            Capturing = true;
        }

        internal void Stop()
        {
            if (Capturing)
            { 
                Capturing = false;
                videoCaptureDevice.Stop();
            }
        }
    }
}
