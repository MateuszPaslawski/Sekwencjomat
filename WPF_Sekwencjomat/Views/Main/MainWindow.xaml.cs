using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace WPF_Sekwencjomat
{

    public partial class MainWindow : Window
    {
        public FilesControl FilesControlObject;
        public SettingsControl SettingsControlObject;
        public PlayerControl PlayerControlObject;

        #region Metody
        private void MakeButtonPressedOnLeft(object sender)
        {
            if (!(sender is Button)) { return; }
            else
            {
                foreach (object item in StackPanel_ButtonsLeft.Children)
                {
                    if (item is Button)
                    {
                        ((Button)item).Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                        ((Button)item).BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    }
                }
                Button btn = sender as Button;
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE1F4FF"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBEE6FD"));
            }
        }

        public void ChangeNavigationVisibility()
        {
            bool condition = 
                (StackPanel_ButtonsLeft.Visibility == Visibility.Visible) 
                && 
                (StackPanel_ButtonsLeft.Visibility == Visibility.Visible);

            if (condition)
            {
                SV_MainDisplay.Content = PlayerControlObject;
                MakeButtonPressedOnLeft(Button_PlayerObjectNavigator);
                Background = new SolidColorBrush(Colors.Black);
                StackPanel_ButtonsLeft.Visibility = Visibility.Collapsed;
                Border_BottomStatusBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Background = new SolidColorBrush(Colors.White);
                StackPanel_ButtonsLeft.Visibility = Visibility.Visible;
                Border_BottomStatusBar.Visibility = Visibility.Visible;
            }
        }

        public void SaveUserSettings()
        {
            try
            {
                Properties.Settings s = Properties.Settings.Default;

                s.VLC_DLL_PATH = SettingsControlObject.TBX_VLCPath.Text;

                s.WINDOW_LOCATION = new Rect(Left, Top, Width, Height);

                if (WindowState == WindowState.Maximized) { s.WINDOWS_MAXIMIZED = true; }
                else { s.WINDOWS_MAXIMIZED = false; }

                s.SETTINGS_ORDER_TAG = Utils.GetPlaybackOrder();

                s.REFVIDEO_PATH = FilesControlObject.TextBox_RefPath.Text;
                s.PAUSEVIDEO_PATH = FilesControlObject.TextBox_PauserPath.Text;

                //s.LIST_OF_FILES = FilesControlObject.CurrentFilesInDataGrid;

                //s.LIST_OF_FILES = new List<string>(FilesControlObject.GetCurrentFilesInDataGrid());
                s.LIST_OF_FILES = FilesControlObject.GetCurrentFilesInDataGrid();
                
                s.Save();
            }
            catch { }
        }

        public async void LoadUserSettings()
        {
            try
            {
                IsEnabled = false;


                Properties.Settings s = Properties.Settings.Default;

                SettingsControlObject.CheckVLCFolderDLLs(s.VLC_DLL_PATH);

                Top = s.WINDOW_LOCATION.Top;
                Left = s.WINDOW_LOCATION.Left;
                Width = s.WINDOW_LOCATION.Width;
                Height = s.WINDOW_LOCATION.Height;

                if (s.WINDOWS_MAXIMIZED) { WindowState = WindowState.Maximized; }

                Utils.SetPlaybackOrder(s.SETTINGS_ORDER_TAG);

                if (File.Exists(s.REFVIDEO_PATH))
                {
                    FilesControlObject.TextBox_RefPath.Text = s.REFVIDEO_PATH;  
                }

                if (File.Exists(s.PAUSEVIDEO_PATH))
                {
                    FilesControlObject.TextBox_PauserPath.Text = s.PAUSEVIDEO_PATH;
                }

                await FilesControlObject.FileDataToGrid(s.LIST_OF_FILES.ToArray());

            }

            catch { }

            finally
            {
                IsEnabled = true;
            }
        }

        #endregion



        public MainWindow()
        {
            Visibility = Visibility.Hidden;
            InitializeComponent();
            
            FilesControlObject = new FilesControl();
            SettingsControlObject = new SettingsControl();
            PlayerControlObject = new PlayerControl();

            SV_MainDisplay.Content = FilesControlObject;
            MakeButtonPressedOnLeft(Button_FileControl);

            LoadUserSettings();

            if (!SettingsControlObject.CheckVLCFolderDLLs(Properties.Settings.Default.VLC_DLL_PATH))
            {
                new SearchingVLCDLLsWindow().ShowDialog();
            }

            Visibility = Visibility.Visible;

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MakeButtonPressedOnLeft(sender);
            SV_MainDisplay.Content = FilesControlObject;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MakeButtonPressedOnLeft(sender);
            SV_MainDisplay.Content = SettingsControlObject;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MakeButtonPressedOnLeft(sender);
            SV_MainDisplay.Content = PlayerControlObject;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveUserSettings();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                ChangeNavigationVisibility();
            }
        }
    }
}
