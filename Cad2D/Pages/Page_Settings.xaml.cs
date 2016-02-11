﻿using Microsoft.Win32;
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
using System.Threading;

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Page_Settings : UserControl
    {
        public EventHandler backPageHandler;
        private CanvasCad2D cc2d;
        public static KeypadTextBox[] registeredTextbox;
        public static bool readingFinished = false;
        public Thread encoderReader;
        private bool firstTime;

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

            registeredTextbox = new KeypadTextBox[17];
            registeredTextbox[0] = textBox_cameraIp;
            registeredTextbox[1] = textBox_plcIp;
            registeredTextbox[2] = textBox_plcPort;
            registeredTextbox[3] = textBox_cameraPort;
            registeredTextbox[4] = textBox_startArrayOffset;
            registeredTextbox[5] = textBox_startVarsOffset;
            registeredTextbox[6] = textBox_sv;
            registeredTextbox[7] = textBox_ev;
            registeredTextbox[8] = textBox_sh;
            registeredTextbox[9] = textBox_eh;
            registeredTextbox[10] = ScanAriaSegment;
            registeredTextbox[11] = VerticalBoundrySegment;
            registeredTextbox[12] = HorizonalBoundrySegment;
            registeredTextbox[13] = textBox_MultY;
            registeredTextbox[14] = textBox_DivY;
            registeredTextbox[15] = textBox_DivX;
            registeredTextbox[16] = textBox_MultX;

            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlcContinoues(CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.valueAddress * 2, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.valueAddress * 2 + 2, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.PackestIdX);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
            encoderReader = new Thread(PlcInfoReaderTimer_Elapsed);
            firstTime = true;
            encoderReader.Start();
        }
        /// <summary>
        /// ///////kar dare
        /// </summary>
        private void PlcInfoReaderTimer_Elapsed()
        {
            if (firstTime)
            {
                Thread.Sleep(2000);
                firstTime = false;
                PlcInfoReaderTimer_Elapsed();
                return;
            }
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD ,CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.valueAddress , ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.readingPacket);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
            else
                return;

            Thread.Sleep(2000);
            PlcInfoReaderTimer_Elapsed();
        }


        public void readPosX()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.readingPacket);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
        }
        public void readPalsY()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.readingPacket);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
        }
        public void readPosY()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.readingPacket);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
        }

        public void readEncoderYValues()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.sendPacketMutex.WaitOne();
                CanvasCad2D.lsConnection.readFromPlcContinoues(CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.valueAddress * 2, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.valueAddress * 2 + 2, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.PackestIdY);
                CanvasCad2D.sendPacketMutex.ReleaseMutex();
            }
        }

        public void OnGUIActions(Action action)
        {
            Dispatcher.Invoke(action);
        }

        public void updateEncoderXValues()
        {
            textBox_PalsX.Text     = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.value.ToString();
            textBox_MultX.Text     = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXMult.value.ToString();
            textBox_DivX.Text      =  CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXDiv.value.ToString();
            textBox_PositionX.Text =  CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.value.ToString();
        }

        public void updateEncoderYValues()
        {
            textBox_PalsY.Text     = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.value.ToString();
            textBox_MultY.Text     = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYMult.value.ToString();
            textBox_DivY.Text      =  CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYDiv.value.ToString();
            textBox_PositionY.Text =  CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.value.ToString();
            readingFinished = true;
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

            s.writeToXmlFile(Env.PrimarySettingsFile);

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
        

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hideKeypad();
        }
        
        public static void hideKeypad()
        {
            foreach (var keypadTextBox in registeredTextbox)
            {
                keypadTextBox.hideKeypad();
            }
        }

        private void button_EncoderY_Click(object sender, RoutedEventArgs e)
        {
            if (readingFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                    int data;
                    if (!Int32.TryParse(textBox_MultY.Text, out data))
                        data = 0;
                    CanvasCad2D.sendPacketMutex.WaitOne();
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYMult.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYMult.writingPacket);
                    CanvasCad2D.sendPacketMutex.ReleaseMutex();
                }
            }
        }

        private void button_EncoderX_Click(object sender, RoutedEventArgs e)
        {
            if (readingFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                    int data;
                    if (!Int32.TryParse(textBox_MultX.Text, out data))
                        data = 0;
                    CanvasCad2D.sendPacketMutex.WaitOne();
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXMult.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXMult.writingPacket);
                    CanvasCad2D.sendPacketMutex.ReleaseMutex();
                }
            }
        }

        public void sendingDivXValueToPlc()
        {
            if (readingFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                    int data;
                    if (!Int32.TryParse(textBox_DivX.Text, out data))
                        data = 0;
                    CanvasCad2D.sendPacketMutex.WaitOne();
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXDiv.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXDiv.writingPacket);
                    CanvasCad2D.sendPacketMutex.ReleaseMutex();
                }
            }
        }
        public void sendingDivYValueToPlc()
        {
            if (readingFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                    int data;
                    if (!Int32.TryParse(textBox_DivY.Text, out data))
                        data = 0;
                    CanvasCad2D.sendPacketMutex.WaitOne();
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYDiv.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYDiv.writingPacket);
                    CanvasCad2D.sendPacketMutex.ReleaseMutex();
                }
            }
        }

        public void setNewChanges()
        {
            textBox_PalsX.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.value.ToString();
            textBox_PositionX.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.value.ToString();
            textBox_PalsY.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.value.ToString();
            textBox_PositionY.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.value.ToString();
        }
    }
}
