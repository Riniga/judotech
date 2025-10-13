using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video;
using Accord.Video.FFMPEG;


namespace Judoka.VideoStreamView
{
    public partial class Form1 : Form
    {
        VideoFileReader reader;
        Size Resolution = new Size(640, 480);
        int FrameRate = 25;
        DateTime lastFrame_TimeStamp;
        Bitmap lastFrame;
        bool playing = false;
        int delay = 3;


        Queue<Bitmap> frames;

        public Form1()
        {
            InitializeComponent();
            reader = new VideoFileReader();
            reader.Open("d:\\test.avi");
            frames = new Queue<Bitmap>();
            new System.Threading.Thread(UpdateBuffer).Start();

            //lastFrame = new Bitmap(Resolution.Width, Resolution.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        }

        private void buttonPlayStop_Click(object sender, EventArgs e)
        {
            playerTimer.Enabled = !playerTimer.Enabled;
            if (playerTimer.Enabled) buttonPlayStop.Text = "&Stop";
            else buttonPlayStop.Text = "&Start";
        }

        private void UpdateBuffer()
        {
            int sleepTime = 1000 / FrameRate;
            while (true)
            {
                var lastFrame = reader.ReadVideoFrame();
                if (lastFrame != null)
                {
                    frames.Append(lastFrame);
                    while (frames.Count > 250) frames.Dequeue();
                }
                System.Threading.Thread.Sleep(sleepTime / FrameRate);
            }
        }

        private void playerTimer_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = frames.FirstOrDefault();
        }
    }
}
