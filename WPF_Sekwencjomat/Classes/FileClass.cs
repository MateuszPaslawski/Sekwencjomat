using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media;

namespace WPF_Sekwencjomat
{
    public class FileClass
    {
        public double FileFPS { get; set; }

        public string FileName { get; set; }

        public string FileColorModel { get; set; }

        public string FileFormat { get; set; }

        public string FileFrameSize { get; set; }

        public string FilePath { get; set; }

        public string FileExtension { get; set; }

        public int FileBitrate { get; set; }

        public string FileSize { get; set; }

        public string FileDuration { get; set; }
    }
}
