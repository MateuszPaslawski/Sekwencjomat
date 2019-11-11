using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System;
using MediaToolkit;
using MediaToolkit.Model;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace WPF_Sekwencjomat
{
    public partial class FilesControl : UserControl
    {

        public List<MediaFile> ListOfMediaFilePropeties = new List<MediaFile>();

        public static List<string> CurrentFilesInDataGrid { get; set; }

        public List<string> GetCurrentFilesInDataGrid()
        {
            return new List<string>(CurrentFilesInDataGrid);
        }

        public void RemoveRowFromDataGrid()
        {
            List<object> listToDelete = new List<object>();
            foreach (object item in DG_Main.SelectedItems)
            {
                MediaFile fc = item as MediaFile;
                CurrentFilesInDataGrid.Remove(fc.Path);
                ListOfMediaFilePropeties.Remove(fc);
                listToDelete.Add(item);
            }
            foreach (var item in listToDelete)
            {
                DG_Main.Items.Remove(item);
            }
        }

        public async Task FileDataToGrid(string[] FilePathList)
        {
            await Task.Run(() =>
            {
                int itemCount = 0;
                TimeSpan timeLeft = new TimeSpan();
                var mediaInfoEngine = new Engine();

                foreach (var item in FilePathList)
                {
                    if (CurrentFilesInDataGrid.Contains(item) || !File.Exists(item)) { continue; }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Dispatcher.Invoke(() =>
                    {
                        Helper.ChangeStatusControl($"Przetwarzanie pliku {++itemCount} / {FilePathList.Length} ({timeLeft.Seconds}s {timeLeft.Milliseconds}ms)", true);
                    });

                    CurrentFilesInDataGrid.Add(item);
                    FileInfo fi = new FileInfo(item);
                    MediaToolkit.Model.MediaFile mf = new MediaToolkit.Model.MediaFile() { Filename = item };

                    using (mediaInfoEngine)
                    {
                        mediaInfoEngine.GetMetadata(mf);
                    }

                    MediaFile mfo = new MediaFile();

                    try
                    {
                        if (mf.Metadata.VideoData.BitRateKbs != null)
                        {
                            mfo.Bitrate = mf.Metadata.VideoData.BitRateKbs.Value;
                        }
                        else
                        {
                            mfo.Bitrate = 0;
                        }
                        mfo.Name = fi.Name;
                        mfo.Path = fi.FullName;
                        mfo.Extension = fi.Extension;
                        mfo.Size = Helper.DecorateBytes(fi.Length);
                        mfo.Format = mf.Metadata.VideoData.Format;
                        mfo.ColorGamut = mf.Metadata.VideoData.ColorModel;
                        mfo.FPS = mf.Metadata.VideoData.Fps;
                        mfo.FrameSize = mf.Metadata.VideoData.FrameSize;
                        mfo.Duration = mf.Metadata.Duration.ToString($"hh\\:mm\\:ss");

                        ListOfMediaFilePropeties.Add(mfo);

                        Dispatcher.Invoke(new Action(() =>
                        {
                            DG_Main.Items.Add(mfo);
                        }));
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Błąd!", $"Nastąpił nieoczekiwany błąd\n\n{exc.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    finally
                    {
                        sw.Stop();
                        timeLeft += sw.Elapsed;
                    }
                }
            });
            Helper.ResetCurrentStatusControl();
        }

        

        public FilesControl()
        {
            InitializeComponent();
            CurrentFilesInDataGrid = new List<string>();
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Multiselect = true, Filter = "Plik Wideo |*.avi; *.mp4; *.mov; *.ogg; *.mkv; *.flv" }; 

            if (fd.ShowDialog() == true)
            {
                await FileDataToGrid(fd.FileNames);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveRowFromDataGrid();
        }

        private void DG_Main_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Multiselect = false, Filter = "Plik Wideo |*.avi; *.mp4; *.mov; *.ogg, *.flv" };
            if (fd.ShowDialog() == true)
            {
                TextBox_RefPath.Text = fd.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Multiselect = false, Filter = "Plik Wideo |*.avi; *.mp4; *.mov; *.ogg, *.flv" };
            if (fd.ShowDialog() == true)
            {
                TextBox_PauserPath.Text = fd.FileName;
            }
        }
    }
}
