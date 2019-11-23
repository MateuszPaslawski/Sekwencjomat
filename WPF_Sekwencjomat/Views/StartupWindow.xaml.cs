using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Threading;

namespace WPF_Sekwencjomat.Views
{
    public partial class StartupWindow : Window
    {
        private List<string> VLC_DLL_SearchList
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

        public StartupWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.MainWindow as MainWindow;
            SettingsControl sc = mw.SettingsControlObject as SettingsControl;

            await Task.Run(() =>
            {
                foreach (var item in VLC_DLL_SearchList)
                {
                    if (sc.CheckVLCFolderDLLs(item))
                        return;
                }
            });
            Close();
        }

    }
}
