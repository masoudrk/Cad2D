using Microsoft.Win32;
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
using SiriusMicrotech.core.UI;

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Page_Settings : UserControl
    {
        public EventHandler backPageHandler;
        private CanvasCad2D cc2d;
        public Page_Settings(CanvasCad2D cc2d)
        {
            InitializeComponent();
            this.cc2d = cc2d;
            load();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void load()
        {
            PrimarySettings s = Extentions.FromXmlPrimary();
            textBox_cameraIp.Text = s.CameraIpAdress;
            textBox_plcIp.Text = s.PLCIpAdress;

            textBox_cameraPort.Text = s.CameraPortNumber.ToString();
            textBox_plcPort.Text = s.PLCPortNumber.ToString();

            textBox_topLeftX.Text = s.TopLeftOffsetX.ToString();
            textBox_topLeftY.Text = s.TopLeftOffsetY.ToString();

            textBox_bottomRightX.Text = s.BottomRightOffsetX.ToString();
            textBox_bottomRightY.Text = s.BottomRightOffsetY.ToString();

            checkBox_showSpeedMenu.IsChecked = s.showSpeedMonitorInMainPanel;
            checkBox_showGuideCircles.IsChecked = s.showGuideCircles;
            checkBox_captureModeInStart.IsChecked = s.captureModeWhenStart;

            textBox_startArrayOffset.Text = s.ArrayMemoryOffset.ToString();
            textBox_startVarsOffset.Text = s.VarMemoryOffset.ToString();

            textBox_sv.Text = s.ScanVerticalSlice.ToString();
            textBox_ev.Text = s.EdgeVerticalSlice.ToString();
            textBox_sh.Text = s.ScanHorizontalSlice.ToString();
            textBox_eh.Text = s.EdgeHorizontalSlice.ToString();

            ScanAriaSegment.Text = s.ScanAriaSegment.ToString();
            VerticalBoundrySegment.Text = s.VerticalBoundrySegment.ToString();
            HorizonalBoundrySegment.Text = s.HorizonalBoundrySegment.ToString();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            int camPort, plcPort;
            IPAddress camIp, plcIp;

            bool camPortValid, plcPortValid;
            camPortValid = int.TryParse(textBox_cameraPort.Text, out camPort);
            plcPortValid = int.TryParse(textBox_plcPort.Text, out plcPort);
            bool camIpValid = IPAddress.TryParse(textBox_cameraIp.Text, out camIp);
            bool plcIpValid = IPAddress.TryParse(textBox_plcIp.Text, out plcIp);

            if (!camIpValid)
            {
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "آی پی وارد شده برای دوربین نامعتبر است");
                return;
            }
            if (!camPortValid)
            {
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "پورت وارد شده برای دوربین نامعتبر است");
                return;
            }
            if (!plcIpValid)
            {
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "آی پی وارد شده PLC نامعتبر است");
                return;
            }
            if (!plcPortValid)
            {
                ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "پورت وارد شده برای PLC نامعتبر است");
                return;
            }

            PrimarySettings s = new PrimarySettings
            {
                CameraPortNumber = camPort,
                PLCPortNumber = plcPort,
                CameraIpAdress = textBox_cameraIp.Text,
                PLCIpAdress = textBox_plcIp.Text,
                TopLeftOffsetX = int.Parse(textBox_topLeftX.Text.ToString()),
                TopLeftOffsetY = int.Parse(textBox_topLeftY.Text.ToString()),
                BottomRightOffsetX = int.Parse(textBox_bottomRightX.Text.ToString()),
                BottomRightOffsetY = int.Parse(textBox_bottomRightY.Text.ToString()),
                showSpeedMonitorInMainPanel = checkBox_showSpeedMenu.IsChecked.Value,
                showGuideCircles = checkBox_showGuideCircles.IsChecked.Value,
                captureModeWhenStart = checkBox_captureModeInStart.IsChecked.Value,
                ArrayMemoryOffset = int.Parse(textBox_startArrayOffset.Text.ToString()),
                VarMemoryOffset = int.Parse(textBox_startVarsOffset.Text.ToString())

            };

            s.ScanVerticalSlice = int.Parse(textBox_sv.Text);
            s.EdgeVerticalSlice = int.Parse(textBox_ev.Text);
            s.ScanHorizontalSlice = int.Parse(textBox_sh.Text);
            s.EdgeHorizontalSlice = int.Parse(textBox_eh.Text);
            s.ScanAriaSegment = int.Parse(ScanAriaSegment.Text);
            s.VerticalBoundrySegment = int.Parse(VerticalBoundrySegment.Text);
            s.HorizonalBoundrySegment = int.Parse(HorizonalBoundrySegment.Text);

            s.writeToXmlFile("_pss.ini");

            ((MainWindow)Application.Current.MainWindow).showMsg("پیام", "تنظیمات با موفقیت ذخیره شدند!");

            backPageHandler?.Invoke("OPTIONS", null);
        }

        private void button_resetOffsets_Click(object sender, RoutedEventArgs e)
        {
            cc2d.border_tools1.Visibility = Visibility.Collapsed;
            cc2d.border_tools2.Visibility = Visibility.Visible;

            ((Image)cc2d.button_switchToCamera.Content).Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Cad2D;component/Resources/tick.png",
                    UriKind.Absolute));
            cc2d.btn_sendToPlc_back.Visibility = Visibility.Collapsed;

            cc2d.pagesStack.Push(this);
            CameraPage cp = new CameraPage();
            cc2d.contentControl.Content = cp;
            cp.start();
        }

        public void setOffsets(Point[] points)
        {
            textBox_topLeftX.Text = points[0].X.ToString();
            textBox_topLeftY.Text = points[0].Y.ToString();

            textBox_bottomRightX.Text = points[1].X.ToString();
            textBox_bottomRightY.Text = points[1].Y.ToString();
        }

        private void textBox_cameraIp_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((KeypadTextBox)sender).hideKeypad();
        }

    }
}
