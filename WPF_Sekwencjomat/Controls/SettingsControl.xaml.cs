using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF_Sekwencjomat
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public bool CheckVLCFolderDLLs(string path)
        {
            bool ret = false;
            Dispatcher.Invoke(() =>
            {
                try
                {
                    ((MainWindow)Application.Current.MainWindow).PlayerControlObject.VLC_Control.SourceProvider.CreatePlayer(new DirectoryInfo(path));
                    TBX_VLCPath.Text = path;
                    IMG_VLCPathStatus.Source = new BitmapImage(new Uri(@"/WPF_Sekwencjomat;component/Resources/icons8-checkmark-20.png", UriKind.Relative));
                    ret = true;
                }
                catch { ret = false; }
            });
            return ret;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!CheckVLCFolderDLLs(dialog.SelectedPath))
                {
                    MessageBox.Show($"Niewłaściwy folder plików programu VLC");
                }
            }
        }
    }
}
