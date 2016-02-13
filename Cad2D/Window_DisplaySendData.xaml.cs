using System;
using System.Collections;
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
using MahApps.Metro.Controls;

namespace Cad2D
{
    /// <summary>
    /// Interaction logic for Window_DisplaySendData.xaml
    /// </summary>
    public partial class Window_DisplaySendData : MetroWindow
    {
        public Window_DisplaySendData(IEnumerable ic)
        {
            InitializeComponent();
            CanvasCad2D.initDataGrid(dataGrid);
            dataGrid.ItemsSource = ic;
        }
        
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
