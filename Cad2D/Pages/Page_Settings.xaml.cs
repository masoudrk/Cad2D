﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using MahApps.Metro.Controls;

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
        public Stack<object> pagesStack;

        public Page_Settings(CanvasCad2D cc2d)
        {
            InitializeComponent();
            pagesStack = new Stack<object>();
            this.cc2d = cc2d;
            load();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void load()
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");

            PrimarySettings s = Extentions.FromXmlPrimary();
            textBox_cameraIp.Text = s.CameraIpAdress;
            textBox_plcIp.Text = s.PLCIpAdress;

            textBox_cameraPort.Text = s.CameraPortNumber.ToString();
            textBox_plcPort.Text = s.PLCPortNumber.ToString();

            textBox_topLeftX.Value = s.TopLeftOffsetX;
            textBox_topLeftY.Value = s.TopLeftOffsetY;

            textBox_bottomRightX.Value = s.BottomRightOffsetX;
            textBox_bottomRightY.Value = s.BottomRightOffsetY;
            
            checkBox_showSpeedMenu.IsChecked = s.showSpeedMonitorInMainPanel;
            checkBox_showGuideCircles.IsChecked = s.showGuideCircles;
            checkBox_captureModeInStart.IsChecked = s.captureModeWhenStart;

            TextBox_Velocity.Text = s.Velocity.ToString();
            TextBox_PlcInformation.Text = s.PlcInformation.ToString();

            textBox_sv.Text = s.ScanVerticalSlice.ToString();
            textBox_ev.Text = string.Format(culture, "{0}", s.EdgeVerticalSlice);
            textBox_sh.Text = s.ScanHorizontalSlice.ToString();
            textBox_eh.Text = s.EdgeHorizontalSlice.ToString();
            
            textBox_evMin.Text = string.Format(culture ,"{0}", s.EdgeVerticalSliceMin) ;
            textBox_evMax.Text = string.Format(culture, "{0}", s.EdgeVerticalSliceMax);
            textBox_ehMin.Text = s.EdgeHorizontalSliceMin.ToString();
            textBox_ehMax.Text = s.EdgeHorizontalSliceMax.ToString();

            TextBox_clampMem.Text = s.ClampMem.ToString();
            TextBox_BridgeOptionMem.Text = s.BridgeOptionMem.ToString();
            TextBox_XEncoderMem.Text = s.XEncoderMem.ToString();
            TextBox_YEncoderMem.Text = s.YEncoderMem.ToString();

            ScanAriaSegment.Text = s.ScanAriaSegment.ToString();
            VerticalBoundrySegment.Text = s.VerticalBoundrySegment.ToString();
            HorizonalBoundrySegment.Text = s.HorizonalBoundrySegment.ToString();

            textBox_FELimit.Text =s.FELimit.ToString();
            textBox_ScaleFESize.Text = s.ScaleFESize.ToString();
            textBox_FocalLinPixels.Text = s.FocalLinPixels.ToString();
            btn_edgeOffset.Value = s.edgeOffset;

            registeredTextbox = new KeypadTextBox[28];
            registeredTextbox[0] = textBox_cameraIp;
            registeredTextbox[1] = textBox_plcIp;
            registeredTextbox[2] = textBox_plcPort;
            registeredTextbox[3] = textBox_cameraPort;
            registeredTextbox[4] = TextBox_Velocity;
            registeredTextbox[5] = TextBox_PlcInformation;
            registeredTextbox[6] = textBox_sv;
            registeredTextbox[8] = textBox_sh;
            registeredTextbox[7] = textBox_ev;
            registeredTextbox[9] = textBox_eh;
            registeredTextbox[10] = ScanAriaSegment;
            registeredTextbox[11] = VerticalBoundrySegment;
            registeredTextbox[12] = HorizonalBoundrySegment;
            registeredTextbox[13] = textBox_MultY;
            registeredTextbox[14] = textBox_DivY;
            registeredTextbox[15] = textBox_DivX;
            registeredTextbox[16] = textBox_MultX;
            registeredTextbox[17] = textBox_evMin;
            registeredTextbox[18] = textBox_ehMin;
            registeredTextbox[19] = textBox_evMax;
            registeredTextbox[20] = textBox_ehMax;
            registeredTextbox[21] = TextBox_XEncoderMem;
            registeredTextbox[22] = TextBox_YEncoderMem;
            registeredTextbox[23] = TextBox_BridgeOptionMem;
            registeredTextbox[24] = TextBox_clampMem;
            registeredTextbox[25] = textBox_FELimit;
            registeredTextbox[26] = textBox_ScaleFESize;
            registeredTextbox[27] = textBox_FocalLinPixels;

            readFirstValues();
            encoderReader = new Thread(PlcInfoReaderTimer_Elapsed);
            encoderReader.Start();
        }

        private void readFirstValues()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                //disk diameter
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.readingPacket);
                //ParkPosAXX
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.readingPacket);
                //ParkPosAXY
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.readingPacket);
                //MinDif
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.MinDif.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.MinDif.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.MinDif.readingPacket);
                //AXXFeedDist
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.readingPacket);
                //AXYFeedDist
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.readingPacket);
                //Hashye
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.Hashye.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.Hashye.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.Hashye.readingPacket);
                //ManSpdAXX
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.readingPacket);
                //ManSpdAXY
                CanvasCad2D.lsConnection.readFromPlc(CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.dataType,
                    CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.valueAddress,
                    ref CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.readingPacket);
            }
        }

        /// <summary>
        /// ///////kar dare
        /// </summary>
        private void PlcInfoReaderTimer_Elapsed()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD ,CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.valueAddress , ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.readingPacket);
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
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.readingPacket);
            }
        }
        public void readPalsY()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                 
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.readingPacket);
                 
            }
        }
        public void readPosY()
        {
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(DataType.WORD, CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.readingPacket);
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
                TopLeftOffsetX = (int)textBox_topLeftX.Value,
                TopLeftOffsetY = (int)textBox_topLeftY.Value,
                BottomRightOffsetX = (int)textBox_bottomRightX.Value,
                BottomRightOffsetY = (int)textBox_bottomRightY.Value,
                showSpeedMonitorInMainPanel = checkBox_showSpeedMenu.IsChecked.Value,
                showGuideCircles = checkBox_showGuideCircles.IsChecked.Value,
                captureModeWhenStart = checkBox_captureModeInStart.IsChecked.Value,
                Velocity = int.Parse(TextBox_Velocity.Text.ToString()),
                PlcInformation = int.Parse(TextBox_PlcInformation.Text.ToString())
            };

            s.ScanVerticalSlice = int.Parse(textBox_sv.Text);
            s.EdgeVerticalSlice = int.Parse(textBox_ev.Text);
            s.ScanHorizontalSlice = int.Parse(textBox_sh.Text);
            s.EdgeHorizontalSlice = int.Parse(textBox_eh.Text);
            s.ScanAriaSegment = int.Parse(ScanAriaSegment.Text);
            s.VerticalBoundrySegment = int.Parse(VerticalBoundrySegment.Text);
            s.HorizonalBoundrySegment = int.Parse(HorizonalBoundrySegment.Text);
            s.EdgeVerticalSliceMin = int.Parse(textBox_evMin.Text);
            s.EdgeVerticalSliceMax = int.Parse(textBox_evMax.Text);
            s.EdgeHorizontalSliceMin = int.Parse(textBox_ehMin.Text);
            s.EdgeHorizontalSliceMax = int.Parse(textBox_ehMax.Text);
            s.BridgeOptionMem = int.Parse(TextBox_BridgeOptionMem.Text);
            s.ClampMem = int.Parse(TextBox_clampMem.Text);
            s.XEncoderMem = int.Parse(TextBox_XEncoderMem.Text);
            s.YEncoderMem = int.Parse(TextBox_YEncoderMem.Text);
            s.FELimit = int.Parse(textBox_FELimit.Text);
            s.ScaleFESize = double.Parse(textBox_ScaleFESize.Text);
            s.FocalLinPixels = int.Parse(textBox_FocalLinPixels.Text);
            if (btn_edgeOffset.Value != null) s.edgeOffset = (int)btn_edgeOffset.Value;
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
            textBox_topLeftX.Value = points[0].X;
            textBox_topLeftY.Value = points[0].Y;

            textBox_bottomRightX.Value = points[1].X;
            textBox_bottomRightY.Value = points[1].Y;
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
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYMult.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYMult.writingPacket);
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
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXMult.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXMult.writingPacket);
                     
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
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXDiv.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXDiv.writingPacket);
                     
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
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, data
                        , CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYDiv.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYDiv.writingPacket);
                     
                }
            }
        }

        public void setNewChanges()
        {
            textBox_PalsX.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPals.value.ToString();
            textBox_PositionX.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderXPos.value.ToString();
            textBox_PalsY.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPals.value.ToString();
            textBox_PositionY.Text = CanvasCad2D.plcUtilitisAndOptions.Encoder.EncoderYPos.value.ToString();

            updateEncoderYValues();
            updateEncoderXValues();
        }

        private void TextBox_Velocity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(TextBox_Velocity.Text, out value))
                label_VelocityTo.Content = $"{value + 1} < ";
            else
                label_VelocityTo.Content = "مقدار وارد شده نامعتبر است";
        }

        private void TextBox_PlcInformation_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(TextBox_PlcInformation.Text, out value))
                label_plcInformationTo.Content = $"{value + 4} < ";
            else
                label_plcInformationTo.Content = "مقدار وارد شده نامعتبر است";
        }

        private void TextBox_clampMem_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(TextBox_clampMem.Text, out value))
                label_clampMem.Content = $"{value + 4} < ";
            else
                label_clampMem.Content = "مقدار وارد شده نامعتبر است";
        }

        private void TextBox_BridgeOptionMem_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(TextBox_BridgeOptionMem.Text, out value))
                label_BridgeOption.Content = $"{value + 7} < ";
            else
                label_BridgeOption.Content = "مقدار وارد شده نامعتبر است";
        }

        private void TextBox_XEncoderMem_TextChanged(object sender, TextChangedEventArgs e)
        { 
            int value = 0;
            if (int.TryParse(TextBox_XEncoderMem.Text, out value))
                label_XEncoder.Content = $"{value + 3} < ";
            else
                label_XEncoder.Content = "مقدار وارد شده نامعتبر است";
        }

        private void TextBox_YEncoderMem_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(TextBox_YEncoderMem.Text, out value))
                label_YEncoder.Content = $"{value + 3} < ";
            else
                label_YEncoder.Content = "مقدار وارد شده نامعتبر است";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //TODO Commented for tehran ray stone
            /*
            CanvasCad2D.pageHscX = new Page_Hsc_X();
            ((TransitioningContentControl)Parent).Content = CanvasCad2D.pageHscX;*/
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {//TODO Commented for tehran ray stone
            /*
            CanvasCad2D.pageHscY = new Page_Hsc_Y();
            ((TransitioningContentControl)Parent).Content = CanvasCad2D.pageHscY;*/
        }

        private void text_Man_Spd_AXY_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.dataType, Int32.Parse(text_Man_Spd_AXY.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ManSpdAXY.writingPacket);
            }
        }

        private void text_Man_Spd_AXX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.dataType, Int32.Parse(text_Man_Spd_AXX.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ManSpdAXX.writingPacket);
            }
        }

        private void text_Hashye_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.Hashye.dataType, Int32.Parse(text_Hashye.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.Hashye.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.Hashye.writingPacket);
            }
        }

        private void text_Feed_Dist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.dataType, Int32.Parse(text_AXY_Feed_Dist.Text)
                           , CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.AXYFeedDist.writingPacket);

            }
        }

        private void text_AXX_Feed_Dist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.dataType, Int32.Parse(text_AXX_Feed_Dist.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.AXXFeedDist.writingPacket);
            }
        }

        private void text_Min_Dif_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.MinDif.dataType, Int32.Parse(text_Min_Dif.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.MinDif.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.MinDif.writingPacket);
            }
        }

        private void text_Park_Pos_AXY_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.dataType, Int32.Parse(text_Park_Pos_AXY.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ParkPosAXY.writingPacket);
            }
        }

        private void text_Park_Pos_AXX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.dataType, Int32.Parse(text_Park_Pos_AXX.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ParkPosAXX.writingPacket);
            }
        }

        private void text_Disk_Diameter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.dataType, Int32.Parse(text_Disk_Diameter.Text)
                        , CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.DiskDiameter.writingPacket);
            }
        }
    }
}
