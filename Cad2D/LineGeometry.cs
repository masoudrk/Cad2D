
using System.Windows.Shapes;
using  System;

namespace Cad2D
{
    public class LineGeometry
    {
        private Line line;
        public double minX, minY, maxX, maxY;
        public double angleX, AngleY;
        private double Tilt, arzAzMabda;
        public bool HorizonalStright { set; get; }
        public bool verticalStright { set; get; }
        public LineGeometry(Line l)
        {
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
            calculateAngleXY();
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
                this.AngleY = (Math.PI/4) - this.angleX;
            }
        }
        private double degreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
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
            }
            else
            {
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
    }
}
