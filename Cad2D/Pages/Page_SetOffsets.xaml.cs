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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Page_SetOffsets.xaml
    /// </summary>
    public partial class Page_SetOffsets : UserControl
    {
        //private CanvasCad2D canvasCad2D;
        public Page_SetOffsets()
        {
            InitializeComponent();

            points = new Point[2];
            _pointCount = 0;
        }
        
        public int _pointCount;
        public Point[] points;

        private void mainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_pointCount < 2)
                {
                    points[_pointCount] = e.GetPosition(mainCanvas);

                    Circle c = CanvasCad2D.cloneCircle();
                    c.X = points[_pointCount].X;
                    c.Y = points[_pointCount].Y;
                    c.mouseActionsEnable = false;

                    Line l1 = CanvasCad2D.cloneLine();
                    l1.Y1 = l1.Y2 = c.Y;
                    l1.X1 = -10;
                    l1.X2 = 10000;

                    Line l2 = CanvasCad2D.cloneLine();
                    l2.X1 = l2.X2 = c.X;
                    l2.Y1 = -10;
                    l2.Y2 = 10000;

                    mainCanvas.Children.Add(l1);
                    mainCanvas.Children.Add(l2);
                    mainCanvas.Children.Add(c);
                    _pointCount++;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                mainCanvas.Children.RemoveRange(1, mainCanvas.Children.Count - 1);
                _pointCount = 0;
            }
        }
    }
}
