using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    class Metrics
    {
        public static double pixelPerUnit = 2;

        public static double clamp(double min, double value, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max; 
            }

            return value;
        }
    }
}
