using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cad2D
{
    public class Analyzer
    {
        private ArrayList mFisheyeCorrect;
        public int mFELimit = 1500;
        public double mScaleFESize = 0.9;
        //private int mFELimit = 1500;
        //private double mScaleFESize = 0.9;
        
        public Analyzer(int mFELimit)
        {
            this.mFELimit = mFELimit;
            //A lookup table so we don't have to calculate Rdistorted over and over
            //The values will be multiplied by focal length in pixels to 
            //get the Rdistorted
            mFisheyeCorrect = new ArrayList(mFELimit);
            //i corresponds to Rundist/focalLengthInPixels * 1000 (to get integers)
            for (int i = 0; i < mFELimit; i++)
            {
                double result = Math.Sqrt(1 - 1 / Math.Sqrt(1.0 + (double)i * i / 1000000.0)) * 1.4142136;
                mFisheyeCorrect.Add(result);
            }
        }

        public Bitmap RemoveFisheye(ref Bitmap aImage, double aFocalLinPixels)
        {
            Bitmap correctedImage = new Bitmap(aImage.Width, aImage.Height);
            //The center points of the image
            double xc = aImage.Width / 2.0;
            double yc = aImage.Height / 2.0;
            Boolean xpos, ypos;
            //Move through the pixels in the corrected image; 
            //set to corresponding pixels in distorted image
            for (int i = 0; i < correctedImage.Width; i++)
            {
                for (int j = 0; j < correctedImage.Height; j++)
                {
                    //which quadrant are we in?
                    xpos = i > xc;
                    ypos = j > yc;
                    //Find the distance from the center
                    double xdif = i - xc;
                    double ydif = j - yc;
                    //The distance squared
                    double Rusquare = xdif * xdif + ydif * ydif;
                    //the angle from the center
                    double theta = Math.Atan2(ydif, xdif);
                    //find index for lookup table
                    int index = (int)(Math.Sqrt(Rusquare) / aFocalLinPixels * 1000);
                    if (index >= mFELimit) index = mFELimit - 1;
                    //calculated Rdistorted
                    double Rd = aFocalLinPixels * (double)mFisheyeCorrect[index]
                                          / mScaleFESize;
                    //calculate x and y distances
                    double xdelta = Math.Abs(Rd * Math.Cos(theta));
                    double ydelta = Math.Abs(Rd * Math.Sin(theta));
                    //convert to pixel coordinates
                    int xd = (int)(xc + (xpos ? xdelta : -xdelta));
                    int yd = (int)(yc + (ypos ? ydelta : -ydelta));
                    xd = Math.Max(0, Math.Min(xd, aImage.Width - 1));
                    yd = Math.Max(0, Math.Min(yd, aImage.Height - 1));
                    //set the corrected pixel value from the distorted image
                    correctedImage.SetPixel(i, j, aImage.GetPixel(xd, yd));
                }
            }
            return correctedImage;
        }
    }
}
