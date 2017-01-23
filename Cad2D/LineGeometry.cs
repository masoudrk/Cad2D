
using System.Windows.Shapes;
using  System;
using System.Drawing;

namespace Cad2D
{
    public class LineGeometry
    {
        public Line line { set; get; }
        public double minX, minY, maxX, maxY;
        public double angleX, AngleY;
        private double Tilt, arzAzMabda;
        private PointF center;
        public double X1, X2, Y1, Y2;
        public int number { set; get; }
        public bool HorizonalStright { set; get; }
        public bool verticalStright { set; get; }
        public LineGeometry(Line l,int number)
        {
            this.number = number;
            minX = (l.X1 < l.X2) ? l.X1 : l.X2;
            minY = (l.Y1 < l.Y2) ? l.Y1 : l.Y2;
            maxX = (l.X1 > l.X2) ? l.X1 : l.X2;
            maxY = (l.Y1 > l.Y2) ? l.Y1 : l.Y2;
            line = l;
            verticalStright = false;
            if (line.Y1 == line.Y2)
                HorizonalStright = true;
            else
                HorizonalStright = false;
            calculateTilt();
            X1 = l.X1;
            X2 = l.X2;
            Y1 = l.Y1;
            Y2 = l.Y2;
            calculateAngleXY();
        }
        public LineGeometry(double anglex, double x, double y)
        {
            this.angleX = anglex;
            if (Math.Abs(anglex - (Math.PI/2)) < 0.00001d)
            {
                verticalStright = true;
                Tilt = 0;
                arzAzMabda = x;
            }else
            if (Math.Abs(anglex) < 0.00001d)
            {
                HorizonalStright = true;
                Tilt = 0;
                arzAzMabda = y;
            }
            else
            {
                Tilt = Math.Tan(anglex);
                arzAzMabda = -(Tilt * x) + y;
            }
            center = new PointF((float)x, (float)y);
        }

        public PointF[] calculateTestPonits(float offset)
        {
            PointF []array = new PointF[2];
            if (verticalStright)
            {
                array[0].X = offset + center.X;
                array[0].Y = center.Y;
                array[1].X = center.X - offset;
                array[1].Y = center.Y;
                return array;
            }
            else if (HorizonalStright)
            {
                array[0].X = center.X;
                array[0].Y = center.Y + offset;
                array[1].X = center.X;
                array[1].Y = center.Y - offset;
                return array;
            }
            else
            {
                double offsetX = Math.Cos(angleX) * offset;
                double offsetY = Math.Sin(angleX) * offset;
                array[0].X = (float)(center.X + offsetX);
                array[0].Y = (float)(center.Y +offsetY);
                array[1].X = (float)(center.X - offsetX);
                array[1].Y = (float)(center.Y - offsetY);
                return array;
            }
        }
        public PointF[] calculatePonits(float offset, double angle)
        {
            PointF[] array = new PointF[2];
            double distance = offset / Math.Sin(angle);

            double offsetX = Math.Cos(angleX) * distance;
            double offsetY = Math.Sin(angleX) * distance;
            array[0].X = (float)(center.X + offsetX);
            array[0].Y = (float)(center.Y + offsetY);
            array[1].X = (float)(center.X - offsetX);
            array[1].Y = (float)(center.Y - offsetY);
            return array;
        }
        private void calculateAngleXY()
        {
            if (HorizonalStright)
            {
                this.angleX = degreesToRadians(0);
                this.AngleY = degreesToRadians(90);
            }else if (verticalStright)
            {
                this.angleX = degreesToRadians(90);
                this.AngleY = degreesToRadians(0);
            }
            else
            {
                this.angleX = Math.Atan(Tilt);
                this.AngleY = Math.Abs((Math.PI / 2) - this.angleX);
            }
        }


        private double degreesToRadians(double degrees)
        {return (Math.PI / 180) * degrees;}

        private void calculateTilt()
        {

            if (line.X1 < line.X2)
            {
                Tilt = (line.Y2 - line.Y1) / (line.X2 - line.X1);
                arzAzMabda = -(Tilt * line.X1) + line.Y1;
            }
            else if (line.X1 > line.X2)
            {
                double tmpx, tmpy;
                tmpx = line.X1;
                tmpy = line.Y1;

                line.X1 = line.X2;
                line.Y1 = line.Y2;
                line.X2 = tmpx;
                line.Y2 = tmpy;

                Tilt = (line.Y2 - line.Y1) / (line.X2 - line.X1);

                arzAzMabda = -(Tilt * line.X1) + line.Y1;
            }else{
                verticalStright = true;
                Tilt = 0;
                arzAzMabda = line.X1;
            }
        }

        public bool checkVerticalColision(double x)
        {
            if (x < minX || x > maxX)
                return false;
            return true;
        }

        public bool checkHorizentalColision(double y)
        {
            if (y < minY || y > maxY)
                return false;
            return true;
        }


        public double calculateLineY(double x)
        {
            if (checkVerticalColision(x))
            {
                if (Tilt == double.MaxValue)
                    return -1;
                return ((Tilt * x) + arzAzMabda);
            }
            else
                return -1;
        }

        public double calculateLineX(double y)
        {
            if (checkHorizentalColision(y))
            {
                if (HorizonalStright)
                    return -1;
                if (verticalStright)
                    return (arzAzMabda);
                return ((y - arzAzMabda) / Tilt);
            }
            else
                return -1;
        }

        public double[] strightVerticalValues()
        {
            if (!verticalStright)
                return null;
            double[] array = new double[2];
            array[0] = minY;
            array[1] = maxY;
            return array;
        }

        public double[] strightHorizentalValues()
        {
            if (!HorizonalStright)
                return null;
            double[] array = new double[2];
            array[0] = minX;
            array[1] = maxX;
            return array;
        }

        internal bool checkVerticalColision(double x, double widthOffsetLength)
        {
            double min, max;
            min = x - widthOffsetLength / 2;
            max = x + widthOffsetLength / 2;

            if (minX < max && minX > min && maxX < max && maxX > min)
                return true;
            return false;
        }
        // // // // /
        internal bool checkHorizentalColision(double y, double widthOffsetLength)
        {
            double min, max;
            min = y - widthOffsetLength / 2;
            max = y + widthOffsetLength / 2;

            if (minY < max && minY > min && maxY < max && maxY > min)
                return true;
            return false;
        }

        static public double CalculateAngle3Point(PointF c, PointF p1, PointF p2 )
        {
            return
                Math.Acos((Math.Pow(CalculateDistance(c, p1), 2) +
                Math.Pow(CalculateDistance(c, p2), 2) -
                          Math.Pow(CalculateDistance(p2, p1), 2)) 
                          / (2 * CalculateDistance(c, p1)* CalculateDistance(c, p2)));
        }
        static public double CalculateDistance(PointF p1, PointF p2)
        {
           return Math.Sqrt(Math.Pow((p1.X - p2.X),2) +Math.Pow((p1.Y - p2.Y),2));
        }
    }
}
