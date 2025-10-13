using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Judoka.VideoStreamCapture
{
    public class VideoStream
    {
        private PictureBox pictureBox;
        private int sleepTime;
        private List<Frame> listofFrames;
        public int CurrentFrame = 0;
        public int NumberOfFrames => listofFrames.Count;
        DateTime start;
        bool playing = false;

        public VideoStream(PictureBox pictureBox, int frameRate)
        {
            this.pictureBox = pictureBox;
            this.sleepTime = 1000 / frameRate;
            listofFrames = new List<Frame>();
            start = DateTime.Now;
        }

        public void AddFrame(Bitmap image)
        {
            new Mirror(false, true).ApplyInPlace(image);
            listofFrames.Add(new Frame { TimeStamp = (DateTime.Now - start).TotalMilliseconds, Image = image } );
        }

        public void Play()
        {
            playing = true;
            while (playing)
            {
                if (listofFrames.Count > CurrentFrame)
                {
                    pictureBox.Image = listofFrames[CurrentFrame].Image;
                }
                double timeStamp = (DateTime.Now - start).TotalMilliseconds;
                while ((listofFrames.Count > CurrentFrame + 1) && listofFrames[CurrentFrame + 1].TimeStamp < timeStamp) CurrentFrame++;
                if (CurrentFrame > 100)
                {
                    listofFrames.RemoveRange(0, CurrentFrame);
                    CurrentFrame = 0;
                }

                System.Threading.Thread.Sleep(sleepTime);
            }
        }

        internal void Start()
        {
            new System.Threading.Thread(Play).Start();
        }

        internal void Stop()
        {
            if (playing)
            { 
                playing = false;
                pictureBox.Image = null;
                System.Threading.Thread.Sleep(sleepTime);
            }
        }
    }
}
