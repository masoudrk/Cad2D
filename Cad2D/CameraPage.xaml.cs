using Hikvision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cad2D
{
    public partial class CameraPage : UserControl
    {
        public HikvisionController hv;

        public CameraPage()
        {
            InitializeComponent();

            PrimarySettings s = Extentions.FromXmlPrimary();

            hv = new HikvisionController(s.CameraIpAdress, s.CameraPortNumber);
            hv.OnConnected += Hv_OnConnected;
            hv.OnDeviceDisconnceted += Hv_OnDeviceDisconnceted;
            hv.OnNewImageCaptured += Hv_OnNewImageCaptured;
            hv.OnStartCapturing += Hv_OnStartCapturing;
            hv.OnStopCapturing += Hv_OnStopCapturing;
            hv.startCapturing();
        }

        private void Hv_OnStopCapturing(object sender, EventArgs e)
        {

        }

        private void Hv_OnStartCapturing(object sender, EventArgs e)
        {

        }

        private void Hv_OnNewImageCaptured(object sender, EventArgs e)
        {
            OnGUIActions(() => image.Source = (BitmapImage)sender);
        }

        private void Hv_OnDeviceDisconnceted(object sender, EventArgs e)
        {
            MessageBox.Show("device Disconnected");
        }

        private void Hv_OnConnected(object sender, EventArgs e)
        {

        }
        
        private void OnGUIActions(Action action)
        {
            Dispatcher.Invoke(action);
        }
        
    }
}
