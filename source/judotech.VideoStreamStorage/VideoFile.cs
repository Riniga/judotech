using Accord.Video.FFMPEG;
using System;
using System.Drawing;

namespace Judoka.VideoStreamStorage
{
    public class VideoFile
    {
        VideoFileWriter writer;
        Size Resolution = new Size(640, 480);
        int FrameRate = 25;
        DateTime lastFrame_TimeStamp;
        Bitmap lastFrame;

        public VideoFile()
        {
            writer = new VideoFileWriter();
            writer.Open("d:\\test.avi", Resolution.Width, Resolution.Height, FrameRate, VideoCodec.MPEG4);
            lastFrame = new Bitmap(Resolution.Width, Resolution.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            lastFrame_TimeStamp = DateTime.Now;
        }

        internal void AddFrame(Bitmap bitmap)
        {
            //Fill
            int numberOfMissingFrames = FrameRate * (int)(DateTime.Now - lastFrame_TimeStamp).TotalMilliseconds / 1000;
            for (int i = 0; i < numberOfMissingFrames; i++) writer.WriteVideoFrame(lastFrame);

            writer.WriteVideoFrame(bitmap);
            lastFrame = bitmap;
            lastFrame_TimeStamp = DateTime.Now;
        }

        internal void Close()
        {
            writer.Close();
        }
    }
}
