using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for Page_SignIn.xaml
    /// </summary>
    public partial class Page_SignIn : UserControl
    {
        private Process keypad;
        private bool adminReq;
        public EventHandler adminBackEvent;
        
        public Page_SignIn(bool adminReq)
        {
            InitializeComponent();
            this.adminReq = adminReq;
        }

        private void showKeyboard()
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) +
                                   "/osk.exe");
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            if (!adminReq)
            {
                RegistryKey key;
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Auth");
                string Pass = key.GetValue("psk").ToString();
                key.Close();

                if (BCrypt.Net.BCrypt.Verify(passBox.Password, Pass))
                {
                    ((TransitioningContentControl) Parent).Content = new CanvasCad2D();
                }
                else
                {
                    passBox.Password = "";
                }
            }
            else
            {
                Cad2D.Properties.Settings ds = Cad2D.Properties.Settings.Default;
                if (BCrypt.Net.BCrypt.Verify(passBox.Password , ds.AdminPass))
                    adminBackEvent.Invoke("95498", null);
                else
                    adminBackEvent.Invoke("8941561", null);
            }
        }

        private void passBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                button_save_Click(null, null);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            passBox.Focus();
        }

        private void imageKeyboard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            showKeyboard();
        }
        
    }
}
