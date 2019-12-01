﻿using MediaToolkit;
using Microsoft.Win32;
using Sekwencjomat.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sekwencjomat.Controls
{
    public partial class FilesControl : UserControl
    {
        public List<MediaFile> ListOfMediaFilsInGrid
        {
            get
            {
                List<MediaFile> tmplist = new List<MediaFile>();
                foreach (object item in DG_Main.Items)
                {
                    tmplist.Add(item as MediaFile);
                }
                return tmplist;
            }
        }
        public List<string> ListOfPathsFilsInGrid
        {
            get
            {
                List<string> tmplist = new List<string>();
                foreach (MediaFile item in ListOfMediaFilsInGrid)
                {
                    tmplist.Add(item.Path);
                }
                return tmplist;
            }
        }

        public void RemoveRowFromDataGrid()
        {
            if (DG_Main.SelectedItems.Count > 1)
            {
                MessageBoxResult dialog = MessageBox.Show($"Czy chcesz usunąć wszystkie zaznaczone pliki z tabeli?\nIlość plików: {DG_Main.SelectedItems.Count}", "Otwieranie eksploratora", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            List<object> listToDelete = new List<object>();
            foreach (object item in DG_Main.SelectedItems)
            {
                MediaFile mf = item as MediaFile;
                listToDelete.Add(item);
            }
            foreach (object item in listToDelete)
            {
                DG_Main.Items.Remove(item);
            }
        }

        public void OpenExplorerFromDataGrid()
        {
            if (DG_Main.SelectedItems.Count > 1)
            {
                MessageBoxResult dialog = MessageBox.Show($"Czy chcesz wskazać wszystkie pliki w eksploratorze?\nIlość plików: {DG_Main.SelectedItems.Count}", "Otwieranie eksploratora", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            foreach (object item in DG_Main.SelectedItems)
            {
                MediaFile mf = item as MediaFile;
                Process.Start("explorer.exe", $"/select, \"{mf.Path}\"");
            }
        }

        public void OpenFileFromDataGrid()
        {
            if (DG_Main.SelectedItems.Count > 1)
            {
                MessageBoxResult dialog = MessageBox.Show($"Czy chcesz odtworzyć wszystkie zaznaczone pliki?\nIlość plików: {DG_Main.SelectedItems.Count}", "Otwieranie eksploratora", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            foreach (object item in DG_Main.SelectedItems)
            {
                MediaFile mf = item as MediaFile;
                Process.Start(mf.Path);
            }
        }


        public async Task FileDataToGrid(string[] FilePathList)
        {
            await Task.Run(() =>
            {
                int itemCount = 0;
                TimeSpan timeLeft = new TimeSpan();
                Engine mediaInfoEngine = new Engine();

                foreach (string item in FilePathList)
                {
                    if (ListOfPathsFilsInGrid.Contains(item) || !File.Exists(item)) { continue; }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Dispatcher.Invoke(() =>
                    {
                        Helper.ChangeStatusControl($"Przetwarzanie pliku {++itemCount} / {FilePathList.Length} ({timeLeft.Seconds}s {timeLeft.Milliseconds}ms)", true);
                    });

                    FileInfo fi = new FileInfo(item);
                    MediaToolkit.Model.MediaFile mf = new MediaToolkit.Model.MediaFile() { Filename = item };

                    using (mediaInfoEngine)
                    {
                        mediaInfoEngine.GetMetadata(mf);
                    }

                    MediaFile mf2 = new MediaFile();

                    try
                    {
                        if (mf.Metadata.VideoData.BitRateKbs != null)
                        {
                            mf2.Bitrate = mf.Metadata.VideoData.BitRateKbs.Value;
                        }
                        else
                        {
                            mf2.Bitrate = 0;
                        }
                        mf2.Name = fi.Name;
                        mf2.Path = fi.FullName;
                        mf2.Extension = fi.Extension;
                        mf2.Size = Helper.DecorateBytes(fi.Length);
                        mf2.Format = mf.Metadata.VideoData.Format;
                        mf2.ColorGamut = mf.Metadata.VideoData.ColorModel;
                        mf2.FPS = mf.Metadata.VideoData.Fps;
                        mf2.FrameSize = mf.Metadata.VideoData.FrameSize;
                        mf2.Duration = mf.Metadata.Duration.ToString($"hh\\:mm\\:ss");

                        Dispatcher.Invoke(new Action(() =>
                        {
                            DG_Main.Items.Add(mf2);
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
            Helper.ResetStatusControl();
        }



        public FilesControl()
        {
            InitializeComponent();
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

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MediaFile item = ((DataGridRow)sender).Item as MediaFile;
            Console.WriteLine(item.Path);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            ContextMenu cm = bt.TemplatedParent as ContextMenu;
            cm.IsOpen = false;
            OpenFileFromDataGrid();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            ContextMenu cm = bt.TemplatedParent as ContextMenu;
            cm.IsOpen = false;
            OpenExplorerFromDataGrid();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            ContextMenu cm = bt.TemplatedParent as ContextMenu;
            cm.IsOpen = false;
            RemoveRowFromDataGrid();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            ContextMenu cm = bt.TemplatedParent as ContextMenu;
            cm.IsOpen = false;
            System.Windows.Controls.Primitives.Popup pp = cm.Parent as System.Windows.Controls.Primitives.Popup;
            DataGridRow dgr = pp.PlacementTarget as DataGridRow;
            MediaFile mf = dgr.Item as MediaFile;

            DG_Main.Items.Remove(mf);
            TextBox_RefPath.Text = mf.Path;
        }
    }
}
