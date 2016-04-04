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

            if (s.alarmBits != null)
            {
                checkBox_alarm1.IsChecked = s.alarmBits[0];
                checkBox_alarm2.IsChecked = s.alarmBits[1];
                checkBox_alarm3.IsChecked = s.alarmBits[2];
                checkBox_alarm4.IsChecked = s.alarmBits[3];
                checkBox_alarm5.IsChecked = s.alarmBits[4];
                checkBox_alarm6.IsChecked = s.alarmBits[5];
                checkBox_alarm7.IsChecked = s.alarmBits[6];
                checkBox_alarm8.IsChecked = s.alarmBits[7];
                checkBox_alarm9.IsChecked = s.alarmBits[8];
                checkBox_alarm10.IsChecked = s.alarmBits[9];
                checkBox_alarm11.IsChecked = s.alarmBits[10];
                checkBox_alarm12.IsChecked = s.alarmBits[11];
            }
            if (s.bridgeTop != null)
            {
                textBox_TopValue.Value = s.bridgeTop.value;
                textBox_TopDelay.Value = s.bridgeTop.delay;
            }
            if (s.bridgeBottom != null)
            {
                textBox_BottomValue.Value = s.bridgeBottom.value;
                textBox_BottomDelay.Value = s.bridgeBottom.delay;
            }
            if (s.bridgeLeft != null)
            {
                textBox_LeftValue.Value = s.bridgeLeft.value;
                textBox_LeftDelay.Value = s.bridgeLeft.delay;
            }
            if (s.bridgeRight != null)
            {
                textBox_RightValue.Value = s.bridgeRight.value;
                textBox_RightDelay.Value = s.bridgeRight.delay;
            }

            textBox_ClampAmount.Value = s.clampAmount;
            textBox_ClampTopLeft.Value = s.clampTopLeft;
            textBox_ClampTopRight.Value = s.clampTopRight;
            textBox_ClampBottomRight.Value = s.clampBottomRight;
            textBox_ClampBottomLeft.Value = s.clampBottomLeft;

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
            s.alarmBits[0] = checkBox_alarm1.IsChecked.Value;
            s.alarmBits[1] = checkBox_alarm2.IsChecked.Value;
            s.alarmBits[2] = checkBox_alarm3.IsChecked.Value;
            s.alarmBits[3] = checkBox_alarm4.IsChecked.Value;
            s.alarmBits[4] = checkBox_alarm5.IsChecked.Value;
            s.alarmBits[5] = checkBox_alarm6.IsChecked.Value;
            s.alarmBits[6] = checkBox_alarm7.IsChecked.Value;
            s.alarmBits[7] = checkBox_alarm8.IsChecked.Value;
            s.alarmBits[8] = checkBox_alarm9.IsChecked.Value;
            s.alarmBits[9] = checkBox_alarm10.IsChecked.Value;
            s.alarmBits[10] = checkBox_alarm11.IsChecked.Value;
            s.alarmBits[11] = checkBox_alarm12.IsChecked.Value;

            s.clampAmount = (int)textBox_ClampAmount.Value;
            s.clampTopLeft = (int)textBox_ClampTopLeft.Value;
            s.clampTopRight = (int)textBox_ClampTopRight.Value;
            s.clampBottomRight = (int)textBox_ClampBottomRight.Value;
            s.clampBottomLeft = (int)textBox_ClampBottomLeft.Value;

            s.bridgeTop = new Settings.Bridge();
            s.bridgeBottom = new Settings.Bridge();
            s.bridgeLeft = new Settings.Bridge();
            s.bridgeRight = new Settings.Bridge();
            s.bridgeTop.value = (int)(textBox_TopValue.Value);
            s.bridgeTop.delay = (int)(textBox_TopDelay.Value);
            s.bridgeBottom.value = (int)(textBox_BottomValue.Value);
            s.bridgeBottom.delay = (int)(textBox_BottomDelay.Value);
            s.bridgeLeft.value = (int)(textBox_LeftValue.Value);
            s.bridgeLeft.delay = (int)(textBox_LeftDelay.Value);
            s.bridgeRight.value = (int)(textBox_RightValue.Value);
            s.bridgeRight.delay = (int)(textBox_RightDelay.Value);


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
            textBox_ClampBottomLeft.Value = CanvasCad2D.plcUtilitisAndOptions.ClampOptions.frontClamp.value;
            readingFromPlcFinished = true;
        }

        public void updateBridgeValues()
        {
            textBox_TopValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.value;
            textBox_TopDelay.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.delay;
            textBox_RightValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.value;
            textBox_RightDelay.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.delay;
            textBox_BottomValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.value;
            textBox_BottomDelay.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.delay;
            textBox_LeftValue.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.value;
            textBox_LeftDelay.Value = CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.delay;
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

        private void textBox_TopDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_TopDelay
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.delayAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetUp.writingPacketDelay);
                     
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

        private void textBox_RightDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_RightDelay
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.delayAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetRight.writingPacketDelay);
                     
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

        private void textBox_BottomDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_BottomDelay
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.delayAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetDown.writingPacketDelay);
                     
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

        private void textBox_LeftDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_LeftDelay
                        .Value, CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.delayAddress, ref CanvasCad2D.plcUtilitisAndOptions.BridgeOptions.stoneOffsetLeft.writingPacketDelay);
                     
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

        private void textBox_ClampBottomLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (readingFromPlcFinished)
            {
                if (CanvasCad2D.lsConnection.Connected)
                {
                     
                    CanvasCad2D.lsConnection.writeToPlc(DataType.WORD, (int)textBox_ClampBottomLeft
                        .Value, CanvasCad2D.plcUtilitisAndOptions.ClampOptions.frontClamp.valueAddress, ref CanvasCad2D.plcUtilitisAndOptions.ClampOptions.frontClamp.writingPacket);
                     
                }
            }
        }
    }
}
