using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Sekwencjomat
{
    public partial class SearchingVLCDLLsWindow : Window
    {
        private List<string> SearchList = new List<string>
        {
            @"C:\Program Files\VLC",
            @"C:\Program Files\VideoLAN",
            @"C:\Program Files\VideoLAN\VLC",
            @"C:\VLC",
            @"C:\VideoLAN",
            @"C:\VideoLAN\VLC",
            @"C:\Program Files (x86)\VLC",
            @"C:\Program Files (x86)\VideoLAN",
            @"C:\Program Files (x86)\VideoLAN\VLC",

            @"D:\Program Files\VLC",
            @"D:\Program Files\VideoLAN",
            @"D:\Program Files\VideoLAN\VLC",
            @"D:\VLC",
            @"D:\VideoLAN",
            @"D:\VideoLAN\VLC",
            @"D:\Program Files (x86)\VLC",
            @"D:\Program Files (x86)\VideoLAN",
            @"D:\Program Files (x86)\VideoLAN\VLC",

            @"E:\Program Files\VLC",
            @"E:\Program Files\VideoLAN",
            @"E:\Program Files\VideoLAN\VLC",
            @"E:\VLC",
            @"E:\VideoLAN",
            @"E:\VideoLAN\VLC",
            @"E:\Program Files (x86)\VLC",
            @"E:\Program Files (x86)\VideoLAN",
            @"E:\Program Files (x86)\VideoLAN\VLC",
        };


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
                    Dispatcher.Invoke(() =>
                    {
                        Label_Main.Content = item;
                    });
                    if (st.CheckVLCFolderDLLs(item))
                    {
                        return;
                    }
                }
            });
            Close();
        }
    }
}
