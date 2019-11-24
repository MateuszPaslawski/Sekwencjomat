using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPF_Sekwencjomat.Models;

namespace WPF_Sekwencjomat
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public void SetHelperPlaybackTechnique(RadioButton rb)
        {
            if (rb.Name == "RadioButton_ACR")
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.ACR;
            else if (rb.Name == "RadioButton_CCR")
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.CCR;
            else if (rb.Name == "RadioButton_DCR")
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.DCR;
            else if (rb.Name == "RadioButton_DCRmod")
                Helper.CurrentPlaybackTechnique = Helper.PlaybackTechnique.DCRmod;
        }

        public void SetHelperPlaybackMode(ComboBoxItem cbi)
        {
            if (cbi.Content.ToString().Contains("Mal"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Descending;
            else if (cbi.Content.ToString().Contains("Ros"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Ascending;
            else if (cbi.Content.ToString().Contains("Los"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Random;
            else if (cbi.Content.ToString().Contains("Wyp"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Convex;
            else if (cbi.Content.ToString().Contains("Wkl"))
                Helper.CurrentPlaybackMode = Helper.PlaybackMode.Concave;
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
            if (sender != null)
                SetHelperPlaybackTechnique(sender as RadioButton);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxItem)((ComboBox)sender).SelectedItem != null)
                SetHelperPlaybackMode((ComboBoxItem)((ComboBox)sender).SelectedItem);
        }

        private void ComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((ComboBoxItem)((ComboBox)sender).SelectedItem != null)
                SetHelperPlaybackMode((ComboBoxItem)((ComboBox)sender).SelectedItem);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, "[^0-9.-]+"))
                e.Handled = true;
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }
    }
}
