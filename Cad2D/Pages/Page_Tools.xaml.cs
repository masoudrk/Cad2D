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
        public static bool readingFromPlcFinished = false;

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

            if (s.bridgeTop != null)
            {
                textBox_TopValue.Value = s.bridgeTop.value;
            }
            if (s.bridgeBottom != null)
            {
                textBox_BottomValue.Value = s.bridgeBottom.value;
            }
            if (s.bridgeLeft != null)
            {
                textBox_LeftValue.Value = s.bridgeLeft.value;
            }
            if (s.bridgeRight != null)
            {
                textBox_RightValue.Value = s.bridgeRight.value;
            }

            textBox_ClampAmount.Value = s.clampAmount;
            textBox_ClampTopLeft.Value = s.clampTopLeft;
            textBox_ClampTopRight.Value = s.clampTopRight;
            textBox_ClampBottomRight.Value = s.clampBottomRight;

            //bridge
             
            if (CanvasCad2D.lsConnection.Connected)
                CanvasCad2D.lsConnection.readFromPlcContinoues(CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.valueAddress * 2, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.delayAddress * 2 + 2, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.PackestId);
             

            //clamp

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
            s.bridgeTop.value = (int)(textBox_TopValue.Value);
            s.bridgeBottom.value = (int)(textBox_BottomValue.Value);
            s.bridgeLeft.value = (int)(textBox_LeftValue.Value);
            s.bridgeRight.value = (int)(textBox_RightValue.Value);


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

        public void updateClampValues()
        {
            textBox_ClampAmount.Value = CanvasCad2D.plcUtilitisAndOptions.ClampOptions.clampValue.value;
            textBox_ClampTopRight.Value = CanvasCad2D.plcUtilitisAndOptions.ClampOptions.downClamp.value;
            textBox_ClampTopLeft.Value = CanvasCad2D.plcUtilitisAndOptions.ClampOptions.upClamp.value;
            textBox_ClampBottomRight.Value = CanvasCad2D.plcUtilitisAndOptions.ClampOptions.behindClamp.value;
            readingFromPlcFinished = true;
        }

        public void updateBridgeValues()
        {
            textBox_TopValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.value;
            textBox_RightValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.value;
            textBox_BottomValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.value;
            textBox_LeftValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.value;
        }

        public void getClampValues()
        {
             
            if (CanvasCad2D.lsConnection.Connected)
                CanvasCad2D.lsConnection.readFromPlcContinoues
                (CanvasCad2D.plcUtilitisAndOptions.ClampOptions.clampValue.valueAddress * 2, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.behindClamp.valueAddress * 2 + 2, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.PackestId);
             
        }

        public void OnGUIActions(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void textBox_TopValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_TopValue
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.writingPacketValue);
                     
                }
            }
        }


        private void textBox_RightValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_RightValue
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.writingPacketValue);
                     
                }
            }
        }


        private void textBox_BottomValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_BottomValue
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.writingPacketValue);
                     
                }
            }

        }



        private void textBox_LeftValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_LeftValue
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.writingPacketValue);
                     
                }
            }

        }

        /// <summary>
        /// //////////clamp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_ClampAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_ClampAmount
                        .Value, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.clampValue.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.clampValue.writingPacket);
                     
                }
            }

        }

        private void textBox_ClampTopRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_ClampTopRight
                        .Value, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.downClamp.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.downClamp.writingPacket);
                     
                }
            }
        }

        private void textBox_ClampTopLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_ClampTopLeft
                        .Value, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.upClamp.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.upClamp.writingPacket);
                     
                }
            }
        }

        private void textBox_ClampBottomRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_ClampBottomRight
                        .Value, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.behindClamp.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.behindClamp.writingPacket);
                     
                }
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
