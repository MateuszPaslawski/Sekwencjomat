using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sekwencjomat.Models
{
    public class MediaFile
    {
        public double FPS { get; set; }

        public string Name { get; set; }

        public string ColorGamut { get; set; }

        public string Format { get; set; }

        public string FrameSize { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

        public int Bitrate { get; set; }

        public string Size { get; set; }

        public string Duration { get; set; }

        public int UserGrade { get; set; }

        public ImageSource IconImage
        {
            get
            {
                ImageSource imageSource;

                Icon icon = Icon.ExtractAssociatedIcon(Path);
                using (Bitmap bmp = icon.ToBitmap())
                {
                    MemoryStream stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    imageSource = BitmapFrame.Create(stream);
                }
                return imageSource;
            }
        }
    }
}
