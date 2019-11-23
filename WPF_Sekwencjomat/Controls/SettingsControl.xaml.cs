using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPF_Sekwencjomat.Models;

namespace WPF_Sekwencjomat
{
    public partial class SettingsControl : UserControl
    {
        App CurrentApp = (App)Application.Current;

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
                    TextBox_VLCPath.Text = path;
                    Image_VLCPathStatus.Source = new BitmapImage(new Uri(@"/WPF_Sekwencjomat;component/Resources/png/checkmark-20.png", UriKind.Relative));
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Name.Contains("ACR"))
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.ACR;
            else if(rb.Name.Contains("CCR"))
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.CCR;
            else if (rb.Name.Contains("DCR"))
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.DCR;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)((ComboBox)sender).SelectedItem;

            if (cbi.Content.ToString().Contains("Mal"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Descending;
            else if (cbi.Content.ToString().Contains("Ros"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Ascending;
            else if (cbi.Content.ToString().Contains("Los"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Random;
        }
    }
}
