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

        private void SetHelperPlaybackTechnique(RadioButton rb)
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

        public void HelperPlaybackPropetiesToControls()
        {
            switch (Helper.CurrentPlaybackTechnique)
            {
                case Helper.PlaybackTechnique.ACR:
                    RadioButton_ACR.IsChecked = true;
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ComboBoxItem_ACR_Descending.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ComboBoxItem_ACR_Ascending.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Concave:
                            ComboBoxItem_ACR_Concave.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Convex:
                            ComboBoxItem_ACR_Convex.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Random:
                            ComboBoxItem_ACR_Random.IsSelected = true;
                            break;
                    }
                    break;

                case Helper.PlaybackTechnique.DCR:
                    RadioButton_DCR.IsChecked = true;
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ComboBoxItem_DCR_Descending.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ComboBoxItem_DCR_Ascending.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Concave:
                            ComboBoxItem_DCR_Concave.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Convex:
                            ComboBoxItem_DCR_Convex.IsSelected = true;
                            break;
                        case Helper.PlaybackMode.Random:
                            ComboBoxItem_DCR_Random.IsSelected = true;
                            break;
                    }
                    break;
            }
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
                catch { }
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
            Console.WriteLine("RadioButton_Checked");
            if (sender != null)
                SetHelperPlaybackTechnique(sender as RadioButton);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("ComboBox_SelectionChanged");
            if ((ComboBoxItem)((ComboBox)sender).SelectedItem != null)
                SetHelperPlaybackMode((ComboBoxItem)((ComboBox)sender).SelectedItem);
        }

        private void ComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("ComboBox_IsEnabledChanged");
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HelperPlaybackPropetiesToControls();
        }
    }
}