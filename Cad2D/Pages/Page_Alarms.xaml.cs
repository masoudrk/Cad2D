using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Page_Alarms.xaml
    /// </summary>
    public partial class Page_Alarms : UserControl
    {
        private string binCode;
        private Action actionOn;
        private Action actionOff;
        private BitmapImage alarmOnBitmap;
        private BitmapImage alarmOffBitmap;

        private Thread alarmThread;

        private Image [] alarmsImage;

        public Page_Alarms()
        {
            InitializeComponent();
            load();

            alarmsImage = new Image[12];
            alarmsImage[0] = alarmImage12;
            alarmsImage[1] = alarmImage11;
            alarmsImage[2] = alarmImage10;
            alarmsImage[3] = alarmImage9;
            alarmsImage[4] = alarmImage8;
            alarmsImage[5] = alarmImage7;
            alarmsImage[6] = alarmImage6;
            alarmsImage[7] = alarmImage5;
            alarmsImage[8] = alarmImage4;
            alarmsImage[9] = alarmImage3;
            alarmsImage[10] = alarmImage2;
            alarmsImage[11] = alarmImage1;


            alarmThread = new Thread(blink) {IsBackground = true};
            alarmThread.Start();

            Dispatcher.ShutdownStarted += OnClosing;
        }

        private void OnClosing(object sender, EventArgs eventArgs)
        {
            alarmThread.Abort();
        }

        private void blink()
        {
            while (true)
            {
                binCode = Utils.GetIntBinaryString(CanvasCad2D.alarm).Substring(20);

                for (int i = 0; i < alarmsImage.Length; i++)
                {
                    if(binCode[i] == '1')
                        Dispatcher.Invoke(new Action(() => alarmsImage[i].Source = alarmOnBitmap));
                }
                Thread.Sleep(300);
                for (int i = 0; i < alarmsImage.Length; i++)
                {
                    Dispatcher.Invoke(new Action(() => alarmsImage[i].Source = alarmOffBitmap));
                }
                Thread.Sleep(300);
            }
        }

        private void load()
        {

            alarmOnBitmap = new BitmapImage(
                new Uri("pack://application:,,,/Cad2D;component/Resources/alarm_on.png",
                    UriKind.Absolute));
            alarmOffBitmap = new BitmapImage(
                new Uri("pack://application:,,,/Cad2D;component/Resources/alarm_off.png",
                    UriKind.Absolute));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            alarmThread.Abort();
        }
    }
}
