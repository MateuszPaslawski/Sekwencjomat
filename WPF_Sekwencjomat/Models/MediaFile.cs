using System.Drawing;
using System.IO;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;


namespace Sekwencjomat.Models
{
    [Serializable]
    [XmlRoot(ElementName = "RatingClass")]
    public class MediaFile
    {
        [XmlElement("FPS")]
        public double FPS { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("ColorGamut")]
        public string ColorGamut { get; set; }

        [XmlElement("Format")]
        public string Format { get; set; }

        [XmlElement("FrameSize")]
        public string FrameSize { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlElement("Extension")]
        public string Extension { get; set; }

        [XmlElement("Bitrate")]
        public int Bitrate { get; set; }

        [XmlElement("Size")]
        public string Size { get; set; }

        [XmlElement("Duration")]
        public string Duration { get; set; }

        [XmlElement("UserGrade")]
        public int UserGrade { get; set; }

        [XmlElement("IconImage")]
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
