using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Hikvision
{
    public class HikvisionController
    {
        IPAddress ip;
        bool capturing;
        public bool Capturing
        {
            get { return capturing; }
            set
            {
                capturing = value;
                if (value && OnStartCapturing != null)
                    OnStartCapturing(this, null);
                else if (!value && OnStopCapturing != null)
                    OnStopCapturing(this, null);
            }
        }

        int portNumber;
        bool connected;
        string cameraUrl;

        public BitmapImage imageCaptured { get; private set; }
        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                if (value && OnConnected != null)
                    OnConnected(this, null);
                else if (!value && OnDeviceDisconnceted != null)
                    OnDeviceDisconnceted(this, null);
            }
        }

        public event EventHandler OnDeviceDisconnceted;
        public event EventHandler OnConnected;
        public event EventHandler OnStartCapturing;
        public event EventHandler OnStopCapturing;
        public event EventHandler OnNewImageCaptured;

        public HikvisionController(string ipAddress , int port)
        {
            this.ip = IPAddress.Parse(ipAddress);
            
            portNumber = port;
            cameraUrl = "http://"+ip.ToString()+":"+port+"/Streaming/channels/1/picture?snapShotImageType=JPEG";
            PingHost(ip);
        }

        public bool startCapturing()
        {
            if (Capturing)
                return false;
            else if(PingHost(ip))
            { Capturing = true; requestFrame();  return true; }
            return false;
        }

        public bool stopCapturing()
        {
            if (!Capturing)
                return false;
            else
            { Capturing = false;
                request.Abort();
                return true; }
        }
        WebRequest request;
        public void requestFrame()
        {
            request = System.Net.HttpWebRequest.Create(cameraUrl);
            request.Credentials = new NetworkCredential("admin", "12345");
            request.Proxy = null;
            request.BeginGetResponse(OnfinishRequestFrame, request);
            
        }
        void OnfinishRequestFrame(IAsyncResult result)
        {
            try
            {
                if (!Capturing) return;
                HttpWebRequest wr = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse )wr.EndGetResponse(result);
                Stream responseStream = response.GetResponseStream();

                byte[] m_Bytes = ReadToEnd(responseStream);
                var bitmap = new BitmapImage();

                using (var stream = new MemoryStream(m_Bytes))
                {

                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                imageCaptured = bitmap;

                if (OnNewImageCaptured != null)
                    OnNewImageCaptured(bitmap, null);

                requestFrame();
            }
            catch(Exception e)
            {
                Thread.Sleep(1000);
                Capturing = false;
                request.Abort();
                startCapturing();
            }
        }

        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        #region ping region

        public bool PingHost(IPAddress nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeOut = 5000;
            PingOptions options = new PingOptions(64, true);
            try
            {
                PingReply reply = pinger.Send(nameOrAddress , timeOut , buffer ,options);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                Connected = false;
                Capturing = false;
                return false;
            }
            Connected = pingable;
            if (pingable == false)
                Capturing = false;
            return pingable;
        }
        public static bool cameraConnection(IPAddress nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            string data = "sagvasvewgdsd";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeOut = 5000;
            PingOptions options = new PingOptions(64, true);
            try
            {
                PingReply reply = pinger.Send(nameOrAddress, timeOut, buffer, options);
                if (reply != null)
                    pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
            return pingable;
        }
        #endregion
    }
}
