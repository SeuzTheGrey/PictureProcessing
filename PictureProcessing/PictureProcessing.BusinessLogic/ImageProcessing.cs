using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureProcessing.BusinessLogic
{
    public static class ImageProcessing
    {
        public static Color GetAverageRGB(Bitmap image)
        {
            //RGB variables for average
            double red = 0;
            double green = 0;
            double blue = 0;

            //number of values
            int numOfValues = 0;

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);

                    red += color.R;
                    green += color.G;
                    blue += color.B;

                    numOfValues++;
                }
            }

            //Average calculation of RGB
            red /= numOfValues;
            green /= numOfValues;
            blue /= numOfValues;

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

        public static double[] ConvertRgbToXyz(Color averageRgb)
        {
            double[] rgb = new double[3];
            double[] xyz = new double[3];

            rgb[0] = (double)averageRgb.R / 255;
            rgb[1] = (double)averageRgb.G / 255;
            rgb[2] = (double)averageRgb.B / 255;

            for (int i = 0; i < rgb.Length; i++)
            {
                if (rgb[i] > 0.04045)
                {
                    rgb[i] = Math.Pow((rgb[i] + 0.055) / 1.055, 2.4);
                }
                else
                {
                    rgb[i] /= 12.92;
                }

                rgb[i] *= 100;
            }

            xyz[0] = rgb[0] * 0.4124 + rgb[1] * 0.3576 + rgb[2] * 0.1805;
            xyz[1] = rgb[0] * 0.2126 + rgb[1] * 0.7152 + rgb[2] * 0.0722;
            xyz[2] = rgb[0] * 0.0193 + rgb[1] * 0.1192 + rgb[2] * 0.9505;

            return xyz;
        }

        public static double[] ConvertXyzToCIELab(double[] xyz)
        {
            double[] lab = new double[3];

            //Using D65 for the illuminator values 
            xyz[0] /= 95.047;
            xyz[1] /= 100.000;
            xyz[2] /= 108.883;

            for (int i = 0; i < xyz.Length; i++)
            {
                if (xyz[i] > 0.008856)
                {
                    //When using Math.POW, Math.Pow(xyz[i], (1/3)) calculates a different value compared to Math.Pow(xyz[i], 0.33333333333). 
                    xyz[i] = Math.Pow(xyz[i], 0.33333333333);
                }
                else
                {
                    xyz[i] = (7.787 * xyz[i]) + (16 / 116);
                }
            }

            lab[0] = (116 * xyz[1]) - 16;
            lab[1] = 500 * (xyz[0] - xyz[1]);
            lab[2] = 200 * (xyz[1] - xyz[2]);

            return lab;
        }

        public static double CalculateDeltaEDifference(double[] lab1, double[] lab2)
        {
            double difference = 0;

            difference = Math.Sqrt(Math.Pow((lab1[0] - lab2[0]), 2) + Math.Pow((lab1[1] - lab2[1]), 2) + Math.Pow((lab1[2] - lab2[2]), 2));

            return difference;
        }
    }
}
