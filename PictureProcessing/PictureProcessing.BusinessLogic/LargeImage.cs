using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PictureProcessing.BusinessLogic
{
    public class LargeImage
    {
        public Bitmap Image { get; set; }
        public Bitmap[,] DividedImage { get; set; }
        public Bitmap[,] NewImage { get; set; }
        public Color[,] AverageRGBOfDividedImages { get; set; }

        public LargeImage()
        {

        }

        public LargeImage(string imageLocation)
        {
            //Prevents the bitmap from locking the original files when in use.
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(imageLocation)))
            {
                Image = new Bitmap(System.Drawing.Image.FromStream(ms));
            }
            
            DividedImage = SplitImage(Image);
            NewImage = new Bitmap[20, 20];

            AverageRGBOfDividedImages = new Color[20,20];

            //Get the average rgb for each divided image
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    AverageRGBOfDividedImages[i, j] = ImageProcessing.GetAverageRGB(DividedImage[i, j]);
                }
            }
        }

        public Bitmap[,] SplitImage(Bitmap image)
        {
            Bitmap[,] dividedImage = new Bitmap[20, 20];

            //The height and width required to make the image into 400 parts
            int splitImageHeight = image.Height / 20;
            int splitImageWidth = image.Width / 20;

            //Counters for the 2d array of bitmaps
            int i = 0;
            int j = 0;

            //split the image and assign the values to the array
            for (int y = 0; y < image.Height; y += splitImageHeight)
            {
                for (int x = 0; x < image.Width; x += splitImageWidth)
                {
                    //incase the width or height exceeds the original image
                    if (x + splitImageWidth > image.Width || y + splitImageHeight > image.Height)
                    {
                        //For medium images that divided in to decimal
                        if (i != 20)
                        {
                            //minus the difference off and apply the new height and width
                            int differenceWidth = (x + splitImageWidth) - image.Width;
                            int differenceHeight = (y + splitImageHeight) - image.Height;

                            Rectangle splitImageRectangle = new Rectangle(x, y, splitImageWidth - differenceWidth, splitImageHeight - differenceHeight);
                            PixelFormat format = image.PixelFormat;

                            dividedImage[i, j] = image.Clone(splitImageRectangle, format);
                        }
                    }
                    else
                    {
                        Rectangle splitImageRectangle = new Rectangle(x, y, splitImageWidth, splitImageHeight);
                        PixelFormat format = image.PixelFormat;

                        dividedImage[i, j] = image.Clone(splitImageRectangle, format);
                    }
                    

                    j++;
                }
                j = 0;
                i++;
            }

            return dividedImage;
        }
    }
}
