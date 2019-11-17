using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Threading;

namespace WPF_Sekwencjomat
{
    public partial class SearchingVLCDLLsWindow : Window
    {
        private List<string> SearchList
        {
            get
            {
                List<string> searchList = new List<string>();

                foreach (DriveInfo item in DriveInfo.GetDrives())
                {
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"VLC"));
                }

                return searchList;
            }
        }

        public SearchingVLCDLLsWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.MainWindow as MainWindow;
            SettingsControl st = mw.SettingsControlObject as SettingsControl;

            await Task.Run(() =>
            {
                foreach (string item in SearchList)
                {
                    if (st.CheckVLCFolderDLLs(item))
                        return;
                }
            });
            Close();
        }
    }
}
