using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureProcessing.BusinessLogic
{
    public class ImageTiles
    {
        public Dictionary<Bitmap,Color> TileImages { get; set; }
        public List<Color> AverageRGB { get; set; }

        public ImageTiles()
        {

        }

        public ImageTiles(string dirLocation)
        {
            TileImages = LoadTileImages(dirLocation);
        }

        private Dictionary<Bitmap, Color> LoadTileImages(string dirLocation)
        {
            Dictionary<Bitmap, Color> tileImages = new Dictionary<Bitmap, Color>();

            string[] files = Directory.GetFiles(dirLocation);

            foreach (string item in files)
            {
                //Prevents the bitmap from locking the original files when in use.
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(item)))
                {
                    tileImages.Add(new Bitmap(Image.FromStream(ms)), ImageProcessing.GetAverageRGB(new Bitmap(Image.FromStream(ms))));
                }
            }

            return tileImages;
        }
    }
}
