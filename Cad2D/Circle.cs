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

namespace Cad2D
{
    public class Circle : Control
    {
        public double X, Y;
        private bool mouseEntered;
        private bool mouseDragged;
        public bool mouseActionsEnable = true;

        public bool isFirstPoint;
        public CanvasCad2D cc2d;
        public Line connectedLine1, connectedLine2;
        public CanvasCad2D.ConnectedLine cl;
        public float radius = 5;

        public Brush defaultColor = Brushes.Blue;

        static Circle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Circle), new FrameworkPropertyMetadata(typeof(Circle)));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (mouseActionsEnable)
            {
                cc2d.onTopCircle = this;
                if (isFirstPoint && !cc2d.connectedsList.loop)
                {
                    cc2d.canLoopPath = true;
                }
                cc2d.mouseEnteredInCircle = true;
                mouseEntered = true;
                InvalidateVisual();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseActionsEnable)
            {
                if (mouseDragged)
                {
                    Point mousePoint = e.GetPosition(cc2d.mainCanvas);
                    X = mousePoint.X;
                    Y = mousePoint.Y;
                }

                if (connectedLine1 != null)
                {
                    connectedLine1.X2 = X;
                    connectedLine1.Y2 = Y;
                }

                if (connectedLine2 != null)
                {
                    connectedLine2.X1 = X;
                    connectedLine2.Y1 = Y;
                }
                InvalidateVisual();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (mouseActionsEnable)
            {
                cc2d.mouseEnteredInCircle = false;

                if (isFirstPoint)
                {
                    cc2d.canLoopPath = false;
                }
                mouseEntered = false;
                InvalidateVisual();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (mouseActionsEnable)
            {
                if (cc2d.state == CanvasCad2D.State.NON)
                {
                    Cursor = Cursors.ScrollAll;
                    mouseDragged = true;
                    cc2d.state = CanvasCad2D.State.DRAGGING_VERTEX;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (mouseActionsEnable)
            {
                if (cc2d.state == CanvasCad2D.State.DRAGGING_VERTEX)
                {
                    mouseDragged = false;
                    cc2d.state = CanvasCad2D.State.NON;
                }

                Cursor = Cursors.Arrow;
            }
            base.OnMouseUp(e);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (mouseDragged)
            {
                dc.DrawEllipse(Brushes.Transparent, new Pen(), new Point(X, Y), 250, 250);
            }

            if (mouseEntered)
            {
                if (isFirstPoint)
                    dc.DrawEllipse(new BrushConverter().ConvertFromString("#55000000") as SolidColorBrush
                        , new Pen(), new Point(X, Y), 10, 10);

                dc.DrawEllipse(Brushes.Red, new Pen(), new Point(X, Y), radius, radius);
            }
            else
                dc.DrawEllipse(defaultColor, new Pen(), new Point(X, Y), radius, radius);

            base.OnRender(dc);
        }
    }
}
