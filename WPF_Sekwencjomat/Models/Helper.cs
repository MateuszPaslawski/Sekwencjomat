using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_Sekwencjomat.Models
{
    class Helper
    {
        public static string GetPlaybackOrder()
        {
            string ret = string.Empty;
            foreach (RadioButton item in ((MainWindow)Application.Current.MainWindow).SettingsControlObject.UniformGrid_PlaybackOrderControl.Children)
            {
                if (item.IsChecked == true)
                    ret = item.Tag.ToString().ToLower();
            }
            return ret;
        }

        public static void SetPlaybackOrder(string order)
        {
            foreach (object item in ((MainWindow)Application.Current.MainWindow).SettingsControlObject.UniformGrid_PlaybackOrderControl.Children)
            {
                if (item is RadioButton)
                {
                    if (((RadioButton)item).Tag.ToString().ToLower() == order)
                        ((RadioButton)item).IsChecked = true;
                }
            }
        }

        public static string DecorateBytes(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB" };
            if (byteCount == 0) { return "0" + suf[0]; }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString() + ' ' + suf[place];
        }

        public static int ResolutionToInt(string resolution)
        {
            int[] ret = { int.Parse(resolution.Split('x').First()), int.Parse(resolution.Split('x').Last()) };
            return ret[0] * ret[1];
        }

        public static void ChangeStatusControl(string info, bool changeCursor)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            if (changeCursor)
            {
                if (mw.Cursor != Cursors.Wait) { mw.Cursor = Cursors.Wait; }
                mw.ProgressBar_Status.Visibility = Visibility.Visible;
            }
            mw.LBL_Status.Content = $"Status [ {info} ]";
        }

        public static void ResetCurrentStatusControl()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Cursor = Cursors.Arrow;
            mw.LBL_Status.Content = $"Status [ Gotowy ]";
            mw.ProgressBar_Status.Visibility = Visibility.Hidden;
        }

        public static SolidColorBrush GetBrushFromHex(string colorString)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
        }

        public static List<T> ShuffleList<T>(List<T> inputList)
        {
            List<T> randomizedList = new List<T>();

            Random r = new Random();
            while (inputList.Count > 0)
            {
                int randomIndex = r.Next(0, inputList.Count);
                randomizedList.Add(inputList[randomIndex]);
                inputList.RemoveAt(randomIndex);
            }

            return randomizedList;
        }

        public static List<MediaFile> AscendingBitrate(List<MediaFile> inputList)
        {
            List<MediaFile> SortedList = inputList.OrderBy(o => o.Bitrate).ToList();
            return SortedList;
        }

        public static List<MediaFile> DescendingBitrate(List<MediaFile> inputList)
        {
            List<MediaFile> SortedList = inputList.OrderByDescending(o => o.Bitrate).ToList();
            return SortedList;
        }

        public static List<MediaFile> AscendingResolution(List<MediaFile> inputList)
        {
            List<MediaFile> SortedList = inputList.OrderBy(o => ResolutionToInt(o.FrameSize)).ToList();
            return SortedList;
        }

        public static List<MediaFile> DescendingResolution(List<MediaFile> inputList)
        {
            List<MediaFile> SortedList = inputList.OrderByDescending(o => ResolutionToInt(o.FrameSize)).ToList();
            return SortedList;
        }

        public static List<MediaFile> SequentialWithRefFile(List<MediaFile> inputList, string referencePath, string pausePath)
        {
            if (referencePath.Trim() == string.Empty || pausePath.Trim() == string.Empty)
            {
                MessageBox.Show($"Brak lub niewłaściwa ścieżka do pliku referencyjnego bądź przerywnika!\n\nPlik referencyjny: {referencePath}\nPlik przerywnika: {pausePath}");
                return null;
            }

            List<MediaFile> first = DescendingBitrate(inputList);
            List<MediaFile> second = new List<MediaFile>(first);
            second.Reverse();
            second.RemoveAt(0);
            List<MediaFile> both = first.Concat(second).ToList();
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in both)
            {
                final.Add(new MediaFile() { Path = referencePath });
                final.Add(item);
                final.Add(new MediaFile() { Path = pausePath });
            }

            return final;
        }
    }
}
