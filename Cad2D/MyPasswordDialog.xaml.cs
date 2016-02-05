using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cad2D
{

    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class MyPasswordDialog : ChildWindow
    {

        public MyPasswordDialog()
        {
            InitializeComponent();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void passBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_save_Click(null, null);
            }
        }
    }
}
