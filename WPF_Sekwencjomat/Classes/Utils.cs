using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPF_Sekwencjomat
{
    class Utils
    {
        public static string GetPlaybackOrder()
        {
            string ret = string.Empty;
            foreach (RadioButton item in ((MainWindow)Application.Current.MainWindow).SettingsControlObject.UG_PlaybackOrderControl.Children)
            {
                if (item.IsChecked == true)
                {
                    ret = item.Tag.ToString().ToLower();
                }
            }
            return ret;
        }

        public static void SetPlaybackOrder(string order)
        {
            foreach (object item in ((MainWindow)Application.Current.MainWindow).SettingsControlObject.UG_PlaybackOrderControl.Children)
            {
                if (item is RadioButton)
                {
                    RadioButton rb = item as RadioButton;
                    if (rb.Tag.ToString().ToLower() == order)
                    {
                        rb.IsChecked = true;
                    }
                }
            }
        }

        public static string BytesToString(long byteCount)
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

        public static List<FileClass> FileClassListAscendingBitrate(List<FileClass> inputList)
        {
            List<FileClass> SortedList = inputList.OrderBy(o => o.FileBitrate).ToList();
            return SortedList;
        }

        public static List<FileClass> FileClassListDescendingBitrate(List<FileClass> inputList)
        {
            List<FileClass> SortedList = inputList.OrderByDescending(o => o.FileBitrate).ToList();
            return SortedList;
        }

        public static List<FileClass> FileClassListAscendingResolution(List<FileClass> inputList)
        {
            List<FileClass> SortedList = inputList.OrderBy(o => ResolutionToInt(o.FileFrameSize)).ToList();
            return SortedList;
        }

        public static List<FileClass> FileClassListDescendingResolution(List<FileClass> inputList)
        {
            List<FileClass> SortedList = inputList.OrderByDescending(o => ResolutionToInt(o.FileFrameSize)).ToList();
            return SortedList;
        }

        public static List<FileClass> FileClassSequentialForRating(List<FileClass> inputList, string reference_path, string pause_path)
        {
            if (reference_path.Trim() == string.Empty || pause_path.Trim() == string.Empty)
            {
                MessageBox.Show($"Brak lub niewłaściwa ścieżka do pliku referencyjnego bądź przerywnika!\n\nPlik referencyjny: {reference_path}\nPlik przerywnika: {pause_path}");
                return null;
            }

            List<FileClass> first = FileClassListDescendingBitrate(inputList);
            List<FileClass> second = new List<FileClass>(first);
            second.Reverse();
            second.RemoveAt(0);
            List<FileClass> both = first.Concat(second).ToList();
            List<FileClass> final = new List<FileClass>();

            foreach (FileClass item in both)
            {
                final.Add(new FileClass() { FilePath = reference_path });
                final.Add(item);
                final.Add(new FileClass() { FilePath = pause_path});
            }

            return final;
        }

        public static List<T> ShuffleList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]);
                inputList.RemoveAt(randomIndex);
            }

            return randomList;
        }

        public static void SetCurrentInfo(string info)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            if (mw.Cursor != Cursors.Wait) { mw.Cursor = Cursors.Wait; }
            mw.LBL_Status.Content = $"Status [ {info} ]";
        }

        public static void SetCurrentInfo(string info, bool changeCursor)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            if (changeCursor)
            {
                if (mw.Cursor != Cursors.Wait) { mw.Cursor = Cursors.Wait; }
                mw.ProgressBar_Status.Visibility = System.Windows.Visibility.Visible;
            }
            mw.LBL_Status.Content = $"Status [ {info} ]";
        }

        public static void ResetCurrentInfo()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Cursor = Cursors.Arrow;
            mw.LBL_Status.Content = $"Status [ Gotowy ]";
            mw.ProgressBar_Status.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
