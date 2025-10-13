using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Runtime.ExceptionServices;

namespace Judoka.VideoStreamCapture
{
    public partial class FormMain : Form
    {
        VideoStream videoStream;
        VideoCapture videoCapture;
        
        static IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        static IPAddress ipAddress = ipHostInfo.AddressList[0];
        static IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11_000);
        Socket socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


        public FormMain()
        {
            InitializeComponent();
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            videoStream = new VideoStream(picture, 30);
            videoCapture = new VideoCapture();
            var inputDevices = videoCapture.VideoInputDevice;
            foreach (FilterInfo filterInfo in inputDevices)
            {
                comboCamera.Items.Add(filterInfo.Name);
            }
            comboCamera.SelectedIndex = 0;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (videoCapture.Capturing)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void Stop()
        {
            videoCapture.Stop();
            videoStream.Stop();
            if(socket!=null)
            { 
                socket.Send(Encoding.UTF8.GetBytes(""), SocketFlags.None);
                socket.Shutdown(SocketShutdown.Both);
                socket = null;
            }

            buttonStart.Text = "&Start Capture";
        }

        private void Start()
        {
            videoCapture.SetCaptureDevice(comboCamera.SelectedIndex);
            socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ipEndPoint);
            videoCapture.Start(VideoCaptureDevice_NewFrame);
            videoStream.Start();
            buttonStart.Text = "&Stop Capturing";
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            var messageBytes = BitMapToByteArray(image);
            socket.Send(messageBytes, SocketFlags.None);

            //if (ones)
            //{
            //    ones = false;
            //    var writer = new BinaryWriter(File.OpenWrite("d:\\formfile.txt"));
            //    writer.Write(messageBytes);

            //}
            videoStream.AddFrame(image);
        }



        public byte[] BitMapToByteArray(Bitmap bitmap)
        {
            Bitmap jpeg = bitmap;
            MemoryStream stream = new MemoryStream();
            jpeg.Save(stream, ImageFormat.Jpeg);
            return stream.ToArray();

            //Encoding.UTF8.GetBytes
            //return Convert.ToBase64String(stream.ToArray());
        }
        public static Bitmap ByteArrayToBitMap(byte[] buffer)
        {
            Stream stream = new MemoryStream(buffer);
            return new Bitmap(stream);
        }



        private void debugTimer_Tick(object sender, EventArgs e)
        {
            framesInBuffer.Text = "Frames in Buffer: " + videoStream.NumberOfFrames;
            pointerPosition.Text = "Pointer position: " + videoStream.CurrentFrame;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }
    }
}
