using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Drawing;


namespace Judoka.VideoStreamStorage
{
    internal class Program
    {
        static IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        static IPAddress ipAddress = ipHostInfo.AddressList[0];
        static IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11_000);

        

        static void Main(string[] args)
        {
            while(true)
            {
                Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen(100);
                var handler = listener.Accept();
                Recieve(handler);
                listener.Close();
            }
        }

        
        static void Recieve(Socket handler)
        {
            VideoFile videoFile = new VideoFile();
            
            while (true)
            {
                var buffer = new byte[32000];
                var received = handler.Receive(buffer);
                if (received == 0) break;

                Array.Resize(ref buffer, received);
                videoFile.AddFrame(ByteArrayToBitMap(buffer));
            }
            videoFile.Close();
        }

        public static Bitmap ByteArrayToBitMap(byte[] buffer)
        {
            Stream stream = new MemoryStream(buffer);
            return new Bitmap(stream);
        }
    }
}
