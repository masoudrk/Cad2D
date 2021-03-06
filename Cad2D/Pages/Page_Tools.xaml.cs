﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Page_Tools : UserControl
    {
        public EventHandler backPageHandler;
        private CanvasCad2D cc2d;
        private bool loaded = false;

        public Page_Tools(CanvasCad2D cc2d)
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
            Settings s = Extentions.FromXml();

            //textBox_ClampAmount.Value = s.clampAmount;
            //textBox_ClampTopLeft.Value = s.clampTopLeft;
            //textBox_ClampTopRight.Value = s.clampTopRight;
            //textBox_ClampBottomRight.Value = s.clampBottomRight;

            textBox_ClampAmount.Value = CanvasCad2D.plcUtilitisAndOptions.ClampAmount.value;
            textBox_ClampTopLeft.Value = CanvasCad2D.plcUtilitisAndOptions.HashyeFront.value;
            textBox_ClampTopRight.Value = CanvasCad2D.plcUtilitisAndOptions.HashyeBack.value;
            textBox_ClampBottomRight.Value = CanvasCad2D.plcUtilitisAndOptions.HashyeEdge.value;

            //bridge

            CanvasCad2D.plcUtilitisAndOptions.getAllClapAndBridge();

            //clamp
            loaded = true;
    }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox_usePass.IsChecked.Value)
            {
                if (textBox_newPass.Password.Equals(textBox_newPassConfirm.Password) &&
                    textBox_newPass.Password.Length > 3)
                {
                    RegistryKey key;
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Auth");
                    if (key != null)
                    {
                        string Pass = key.GetValue("psk").ToString();
                        key.Close();

                        if (BCrypt.Net.BCrypt.Verify(textBox_nowPass.Password, Pass))
                        {
                            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Auth");
                            key.SetValue("psk", BCrypt.Net.BCrypt.HashPassword(textBox_newPass.Password));
                            key.Close();
                        }
                    }
                    else
                    {
                        key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Auth");
                        key.SetValue("psk", BCrypt.Net.BCrypt.HashPassword(textBox_newPass.Password));
                        key.Close();
                    }
                }
                else
                {
                    if (!textBox_newPass.Password.Equals(""))
                        if (!(textBox_newPass.Password.Length > 3))
                            ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "طول رمز جدید میبایستی بیشتر از 4 کاراکتر باشد.");
                        else
                            ((MainWindow)Application.Current.MainWindow).showMsg("خطا", "رمزهای جدید وارد شده با هم یکسان نیستند.");
                }
            }
            else
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Auth");

                if (rk != null)
                {
                    rk.Close();
                    Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Auth");
                }
            }

            Settings s = new Settings();
            s.alarmBits = new bool[12];

            s.clampAmount = (int)textBox_ClampAmount.Value;
            s.clampTopLeft = (int)textBox_ClampTopLeft.Value;
            s.clampTopRight = (int)textBox_ClampTopRight.Value;
            s.clampBottomRight = (int)textBox_ClampBottomRight.Value;

            s.bridgeTop = new Settings.Bridge();
            s.bridgeBottom = new Settings.Bridge();
            s.bridgeLeft = new Settings.Bridge();
            s.bridgeRight = new Settings.Bridge();

            s.writeToXmlFile(Env.SettingsFile);

            ((MainWindow)Application.Current.MainWindow).showMsg("پیام", "تنظیمات با موفقیت ذخیره شدند!");

            if (backPageHandler != null)
                backPageHandler.Invoke("TOOLS", null);
        }

        private void checkBox_usePass_Checked(object sender, RoutedEventArgs e)
        {
            setUsePassword(true);
        }

        private void checkBox_usePass_Unchecked(object sender, RoutedEventArgs e)
        {
            setUsePassword(false);
        }

        public void setUsePassword(bool use)
        {
            panel_pass.IsEnabled = use;
            panel_newPass.IsEnabled = use;
            panel_newConfirmPass.IsEnabled = use;
        }

        private void imageKeyboard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) +
                                   "/osk.exe");
        }
        
        /// <summary>
        /// //////////clamp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_ClampAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (CanvasCad2D.lsConnection.Connected && loaded)
            {
                if (textBox_ClampAmount.Value != null)
                    CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.ClampAmount.dataType, (int)textBox_ClampAmount.Value * 10,
                        CanvasCad2D.plcUtilitisAndOptions.ClampAmount.valueAddress,
                        ref CanvasCad2D.plcUtilitisAndOptions.ClampAmount.writingPacket);
            }
        }

        private void textBox_ClampTopRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (CanvasCad2D.lsConnection.Connected && loaded)
            {
                if (textBox_ClampTopRight.Value != null)
                    CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.HashyeBack.dataType, (int)textBox_ClampTopRight.Value * 10,
                        CanvasCad2D.plcUtilitisAndOptions.HashyeBack.valueAddress,
                        ref CanvasCad2D.plcUtilitisAndOptions.HashyeBack.writingPacket);
            }
        }

        private void textBox_ClampTopLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (CanvasCad2D.lsConnection.Connected && loaded)
            {
                if (textBox_ClampTopLeft.Value != null)
                    CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.HashyeFront.dataType, (int)textBox_ClampTopLeft.Value * 10,
                        CanvasCad2D.plcUtilitisAndOptions.HashyeFront.valueAddress,
                        ref CanvasCad2D.plcUtilitisAndOptions.HashyeFront.writingPacket);
            }
        }

        private void textBox_ClampBottomRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {//edge
            if (CanvasCad2D.lsConnection.Connected && loaded)
            {
                if (textBox_ClampBottomRight.Value != null)
                    CanvasCad2D.lsConnection.writeToPlc(CanvasCad2D.plcUtilitisAndOptions.HashyeEdge.dataType, (int)textBox_ClampBottomRight.Value * 10,
                        CanvasCad2D.plcUtilitisAndOptions.HashyeEdge.valueAddress,
                        ref CanvasCad2D.plcUtilitisAndOptions.HashyeEdge.writingPacket);
            }
        }

        private void button_Scroll_Down_Click(object sender, RoutedEventArgs e)
        {
            PageScroller.ScrollToVerticalOffset(PageScroller.VerticalOffset + 100);
        }

        private void button_Scroll_Up_Click(object sender, RoutedEventArgs e)
        {
            PageScroller.ScrollToVerticalOffset(PageScroller.VerticalOffset - 100);
        }
    }
}
