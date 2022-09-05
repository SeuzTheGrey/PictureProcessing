using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using PictureProcessing.BusinessLogic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PictureProcessing.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ProcessImages(HttpPostedFileBase file)
        {
            //Check if the upload button was accidently clicked
            if (file == null)
            {
                return View("Index");
            }
            LargeImage largeImage = new LargeImage();

            ImageTiles tiles = new ImageTiles();

            Parallel.Invoke(() =>
            {
                string image = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~"), image);
                // file is uploaded
                file.SaveAs(path);
                //Process Image retrieved from frontend
                largeImage = new LargeImage(path);
            },
            () =>
            {
                //Process image tiles and calculate average rgb
                tiles = new ImageTiles(@"D:\Projects\PictureProcessing\PictureProcessing\101_ObjectCategories\101_ObjectCategories\bass");
            });

            await Task.Run(() =>
            {
                //Cycle through the columns and rows of Divided Image to find an appropriate tile image for each
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        double difference = 1000;
                        double deltaValue = 1000;
                        //Cycles through the tile images for the current column and row
                        foreach (KeyValuePair<Bitmap, Color> item in tiles.TileImages)
                        {
                            //The difference calculated for the two lab values
                            deltaValue = CalculateDifference(largeImage.AverageRGBOfDividedImages[i, j], item.Value);
                            if (deltaValue <= difference && deltaValue >= 0)
                            {
                                difference = deltaValue;
                                largeImage.NewImage[i, j] = item.Key;
                            }
                        }
                    }
                }

                //Final Image with the original image width and height
                Bitmap finalImage = new Bitmap(largeImage.Image.Width, largeImage.Image.Height);

                //The height and width required to make the image into 400 parts
                int imageHeight = finalImage.Height / 20;
                int imageWidth = finalImage.Width / 20;

                //Counters for the 2d array of bitmaps
                int row = 0;
                int column = 0;

                //assign the new chossen images to the locations
                for (int y = 0; y < finalImage.Height; y += imageHeight)
                {
                    for (int x = 0; x < finalImage.Width; x += imageWidth)
                    {
                        //Assign the final image to graphics for drawing
                        using (Graphics graphics = Graphics.FromImage(finalImage))
                        {
                            //For medium images that divided in to decimal
                            if (row != 20)
                            {
                                //Draw the closest images in the specified locations
                                graphics.DrawImage(largeImage.NewImage[row, column], x, y, imageWidth, imageHeight);
                            }
                        }

                        column++;
                    }
                    column = 0;
                    row++;
                }

                finalImage.Save("D:\\Projects\\PictureProcessing\\PictureProcessing\\PictureProcessing.UI\\finalImage.jpg", ImageFormat.Jpeg);
            });


            return View();
        }

        public double CalculateDifference(Color dividedImage, Color imageTile)
        {
            //Convert the values from rgb to xyz
            double[] lab1 = ImageProcessing.ConvertRgbToXyz(dividedImage);
            double[] lab2 = ImageProcessing.ConvertRgbToXyz(imageTile);

            //Convert the values from xyz to CIE lab
            lab1 = ImageProcessing.ConvertXyzToCIELab(lab1);
            lab2 = ImageProcessing.ConvertXyzToCIELab(lab2);

            //Calculate and return the difference between the two colors
            return ImageProcessing.CalculateDeltaEDifference(lab1, lab2);
        }
    }
}