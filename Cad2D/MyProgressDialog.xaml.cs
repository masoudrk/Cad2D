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
using MahApps.Metro.Controls;

namespace Cad2D
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class MyProgressDialog : ChildWindow
    {
        public MyProgressDialog()
        {
            InitializeComponent();
        }

        public void setProgressValues(int total, int stoneScan, int horizontalPoints, int verticalPoints , int innerPoints)
        {
            progressBar.Value = total;
            this.stoneScan.Value = stoneScan;
            this.horizontalPoints.Value = horizontalPoints;
            this.verticalPoints.Value = verticalPoints;
            this.innerPoints.Value = innerPoints;
        }
    }
}
