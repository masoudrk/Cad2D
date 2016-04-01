using MahApps.Metro.Controls;
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
    public partial class MyStartDirectionDialog : ChildWindow

    {
        public int direction { set; get; }

        public MyStartDirectionDialog()
        {
            direction = 0;
            InitializeComponent();
        }

        private void button_Dir_1_2_Click(object sender, RoutedEventArgs e)
        {
            direction = 1;
            this.Close();
        }

        private void button_Dir_1_4_Click(object sender, RoutedEventArgs e)
        {
            direction = 2;
            this.Close();
        }

        private void button_Dir_2_1_Click(object sender, RoutedEventArgs e)
        {
            direction = 3;
            this.Close();
        }

        private void button_Dir_2_3_Click(object sender, RoutedEventArgs e)
        {
            direction = 4;
            this.Close();
        }

        private void button_Dir_3_2_Click(object sender, RoutedEventArgs e)
        {
            direction = 5;
            this.Close();
        }

        private void button_Dir_3_4_Click(object sender, RoutedEventArgs e)
        {
            direction = 6;
            this.Close();
        }

        private void button_Dir_4_1_Click(object sender, RoutedEventArgs e)
        {
            direction = 7;
            this.Close();
        }

        private void button_Dir_4_3_Click(object sender, RoutedEventArgs e)
        {
            direction = 8;
            this.Close();
        }

        private void button_Cancle_Click(object sender, RoutedEventArgs e)
        {
            direction = 1000;
            this.Close();
        }

        private void button_Edges_Click(object sender, RoutedEventArgs e)
        {
            direction = 9;
            this.Close();
        }
    }
}
