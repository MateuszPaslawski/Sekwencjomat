using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_Sekwencjomat.Models
{
    public static class Helper
    {
        public static PlaybackTechnique CurrentPlaybackTechnique;
        public static PlaybackMode CurrentPlaybackMode;
        public static bool IsInterfaceLocked = false;
        public static string ExecutionPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).ToString();
            }
        }

        public static int RatingDelay
        {
            get
            {
                return int.Parse(((MainWindow)Application.Current.MainWindow).SettingsControlObject.TextBox_RatingDelay.Text);
            }
            set
            {
                ((MainWindow)Application.Current.MainWindow).SettingsControlObject.TextBox_RatingDelay.Text = value.ToString();
            }
        }

        public enum PlaybackTechnique
        {
            ACR,
            CCR,
            DCR,
            DCRmod,
            Unknown,
        }

        public enum PlaybackMode
        {
            Descending,
            Ascending,
            Concave,
            Convex,
            Random,
            Unknown,
        }


        public static string PlaybackModeToString(PlaybackMode playbackMode)
        {
            string ret = "Nieznany";

            switch (playbackMode)
            {
                case PlaybackMode.Descending:
                    ret = "Malejąco";
                    break;
                case PlaybackMode.Ascending:
                    ret = "Rosnąco";
                    break;
                case PlaybackMode.Concave:
                    ret = "Wklęsło";
                    break;
                case PlaybackMode.Convex:
                    ret = "Wypukło";
                    break;
                case PlaybackMode.Random:
                    ret = "Losowo";
                    break;
            }

            return ret;
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

        public static SolidColorBrush GetBrushFromHex(string colorString)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
        }

        public static bool IsDirectoryEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
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

        public static void ResetStatusControl()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Cursor = Cursors.Arrow;
            mw.LBL_Status.Content = $"Status [ Gotowy ]";
            mw.ProgressBar_Status.Visibility = Visibility.Hidden;
        }

        public static void DisableNavigationButtons()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.StackPanel_ButtonsLeft.IsEnabled = false;
        }

        public static void EnableNavigationButtons()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.StackPanel_ButtonsLeft.IsEnabled = true;
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



        public static List<MediaFile> ACR_Concave(List<MediaFile> inputList)
        {
            List<MediaFile> first = DescendingBitrate(inputList);
            List<MediaFile> second = new List<MediaFile>(first);
            second.Reverse();
            second.RemoveAt(0);
            List<MediaFile> final = first.Concat(second).ToList();

            return final;
        }

        public static List<MediaFile> ACR_Convex(List<MediaFile> inputList)
        {
            List<MediaFile> first = AscendingBitrate(inputList);
            List<MediaFile> second = new List<MediaFile>(first);
            second.Reverse();
            second.RemoveAt(0);
            List<MediaFile> final = first.Concat(second).ToList();

            return final;
        }

        public static List<MediaFile> ACR_Random(List<MediaFile> inputList)
        {
            List<MediaFile> first = ShuffleList(inputList);
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in first)
                final.Add(item);

            return final;
        }

        public static List<MediaFile> ACR_Ascending(List<MediaFile> inputList)
        {
            return AscendingBitrate(inputList);
        }

        public static List<MediaFile> ACR_Descending(List<MediaFile> inputList)
        {
            return DescendingBitrate(inputList);
        }



        public static List<MediaFile> DCR_Concave(List<MediaFile> inputList, string referencePath)
        {
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
            }

            return final;
        }
        
        public static List<MediaFile> DCR_Convex(List<MediaFile> inputList, string referencePath)
        {
            List<MediaFile> first = AscendingBitrate(inputList);
            List<MediaFile> second = new List<MediaFile>(first);
            second.Reverse();
            second.RemoveAt(0);
            List<MediaFile> both = first.Concat(second).ToList();
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in both)
            {
                final.Add(new MediaFile() { Path = referencePath });
                final.Add(item);
            }

            return final;
        }

        public static List<MediaFile> DCR_Random(List<MediaFile> inputList, string referencePath)
        {
            List<MediaFile> first = ShuffleList(inputList);
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in first)
            {
                final.Add(new MediaFile() { Path = referencePath });
                final.Add(item);
            }

            return final;
        }

        public static List<MediaFile> DCR_Ascending(List<MediaFile> inputList, string referencePath)
        {
            List<MediaFile> first = AscendingBitrate(inputList);
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in first)
            {
                final.Add(new MediaFile() { Path = referencePath });
                final.Add(item);
            }

            return final;

        }

        public static List<MediaFile> DCR_Descending(List<MediaFile> inputList, string referencePath)
        {
            List<MediaFile> first = DescendingBitrate(inputList);
            List<MediaFile> final = new List<MediaFile>();

            foreach (MediaFile item in first)
            {
                final.Add(new MediaFile() { Path = referencePath });
                final.Add(item);
            }

            return final;
        }


    }
}
