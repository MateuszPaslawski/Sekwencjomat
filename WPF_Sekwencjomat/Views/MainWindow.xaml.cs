using Sekwencjomat.Controls;
using Sekwencjomat.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sekwencjomat
{

    public partial class MainWindow : Window
    {
        public FilesControl FilesControlObject;
        public PlayerControl PlayerControlObject;
        public SettingsControl SettingsControlObject;
        public UserRatingControl UserRatingControlObject;
        public Rect WindowsRectBeforeFullScreen;
        public WindowState WindowStateBeforeFullScreen;

        private List<string> VLC_DLL_SearchList
        {
            get
            {
                List<string> searchList = new List<string>();
                string ProgramFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string ProgramFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                searchList.Add(Path.Combine(ProgramFilesPath, @"VLC"));
                searchList.Add(Path.Combine(ProgramFilesPath, @"VideoLAN"));
                searchList.Add(Path.Combine(ProgramFilesPath, @"VideoLAN\VLC"));

                searchList.Add(Path.Combine(ProgramFilesX86Path, @"VLC"));
                searchList.Add(Path.Combine(ProgramFilesX86Path, @"VideoLAN"));
                searchList.Add(Path.Combine(ProgramFilesX86Path, @"VideoLAN\VLC"));

                foreach (DriveInfo item in DriveInfo.GetDrives())
                {

                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files (x86)\VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"Program Files\VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"VideoLAN\VLC"));
                    searchList.Add(Path.Combine(item.ToString(), @"VideoLAN"));
                    searchList.Add(Path.Combine(item.ToString(), @"VLC"));
                }
                return searchList;
            }
        }

        #region Metody Użytkownika
        private void MakeButtonPressedOnLeft(object sender)
        {
            if (!(sender is Button)) { return; }
            else
            {
                foreach (object item in StackPanel_ButtonsLeft.Children)
                {
                    if (item is Button)
                    {
                        ((Button)item).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                        ((Button)item).BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    }
                }
                Button btn = sender as Button;
                btn.Background = Helper.GetBrushFromHex("#FFE1F4FF");
                btn.BorderBrush = Helper.GetBrushFromHex("#FFBEE6FD");
            }
        }

        public void SwitchFullScreen(bool makeVisible)
        {
            bool condition =
                (StackPanel_ButtonsLeft.Visibility == Visibility.Visible)
                &&
                (Border_BottomStatusBar.Visibility == Visibility.Visible);

            if (condition && !makeVisible)
            {
                SV_MainDisplay.Content = PlayerControlObject;
                MakeButtonPressedOnLeft(Button_PlayerObjectNavigator);
                Background = new SolidColorBrush(Colors.Black);
                StackPanel_ButtonsLeft.Visibility = Visibility.Collapsed;
                Border_BottomStatusBar.Visibility = Visibility.Collapsed;
                WindowsRectBeforeFullScreen = new Rect
                {
                    Height = ActualHeight,
                    Width = ActualWidth,
                    Y = Top,
                    X = Left,
                };
                WindowStateBeforeFullScreen = WindowState;

                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                Height = WindowsRectBeforeFullScreen.Height;
                Width = WindowsRectBeforeFullScreen.Width;
                Top = WindowsRectBeforeFullScreen.Y;
                Left = WindowsRectBeforeFullScreen.X;
                WindowState = WindowStateBeforeFullScreen;

                Background = new SolidColorBrush(Colors.White);
                StackPanel_ButtonsLeft.Visibility = Visibility.Visible;
                Border_BottomStatusBar.Visibility = Visibility.Visible;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        public async Task SearchForVLCDLL()
        {
            await Task.Run(() =>
            {
                foreach (string item in VLC_DLL_SearchList)
                {
                    if (!Directory.Exists(item))
                        continue;

                    if (SettingsControlObject.CheckVLCFolderDLLs(item))
                    {
                        PlayerControlObject.VLC_Control.SourceProvider.CreatePlayer(new DirectoryInfo(item));
                        return;
                    }
                }
            });
        }

        public async Task LoadUserSettings()
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                return;
            }

            try
            {
                Properties.Settings settings = Properties.Settings.Default;

                //Check if saved VLC Path is valid
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() => 
                    {

                        //Window Location and State
                        if (settings.WINDOW_LOCATION.Top < SystemParameters.WorkArea.Height && settings.WINDOW_LOCATION.Left < SystemParameters.WorkArea.Width)
                        {
                            Top = settings.WINDOW_LOCATION.Top;
                            Left = settings.WINDOW_LOCATION.Left;
                            Width = settings.WINDOW_LOCATION.Width;
                            Height = settings.WINDOW_LOCATION.Height;
                        }

                        WindowState = settings.WINDOW_STATE;

                        //Reference video path
                        if (File.Exists(settings.REFVIDEO_PATH))
                            FilesControlObject.TextBox_RefPath.Text = settings.REFVIDEO_PATH;

                        //RatingDelay
                        Helper.RatingDelay = settings.RATING_DELAY;
                        SettingsControlObject.CheckVLCFolderDLLs(settings.VLC_DLL_PATH);
                

                        //Fill DataGrid with all files
                        FilesControlObject.FileDataToGrid(settings.LIST_OF_FILES.ToArray());


                        //PlaybackScale
                        switch (settings.PLAYBACK_TECHNIQUE.ToLower())
                        {
                            case "acr":
                                Helper.CurrentPlaybackScale = Helper.PlaybackScaleEnum.ACR;
                                break;
                            case "dcr":
                                Helper.CurrentPlaybackScale = Helper.PlaybackScaleEnum.DCR;
                                break;
                        }

                        //PlaybackMode
                        switch (settings.PLAYBACK_MODE.ToLower())
                        {
                            case "ascending":
                                Helper.CurrentPlaybackMode = Helper.PlaybackModeEnum.Ascending;
                                break;
                            case "descending":
                                Helper.CurrentPlaybackMode = Helper.PlaybackModeEnum.Descending;
                                break;
                            case "random":
                                Helper.CurrentPlaybackMode = Helper.PlaybackModeEnum.Random;
                                break;
                            case "convex":
                                Helper.CurrentPlaybackMode = Helper.PlaybackModeEnum.Convex;
                                break;
                            case "concave":
                                Helper.CurrentPlaybackMode = Helper.PlaybackModeEnum.Concave;
                                break;
                        }

                        //Set different controls propeties from settings
                        SettingsControlObject.HelperPlaybackPropetiesToControls();
                    });
                });
            }
            catch { }
        }

        private async Task WarmupMediaInfoEngine()
        {
            await Task.Run(() =>
            {
                new MediaToolkit.Engine();
            });
        }

        public void SaveUserSettings()
        {
            try
            {
                Properties.Settings settings = Properties.Settings.Default;
                settings.WINDOW_STATE = WindowState;
                settings.VLC_DLL_PATH = SettingsControlObject.TextBox_VLCPath.Text;
                settings.WINDOW_LOCATION = new Rect(Left, Top, Width, Height);
                settings.REFVIDEO_PATH = FilesControlObject.TextBox_RefPath.Text;
                settings.LIST_OF_FILES = FilesControlObject.ListOfPathsFilsInGrid;
                settings.PLAYBACK_MODE = Helper.CurrentPlaybackMode.ToString();
                settings.PLAYBACK_TECHNIQUE = Helper.CurrentPlaybackScale.ToString();
                settings.RATING_DELAY = Helper.RatingDelay;
                settings.Save();
            }
            catch { }
        }
        #endregion



        public MainWindow()
        {
            InitializeComponent();
            string architecture = Assembly.GetEntryAssembly().GetName().ProcessorArchitecture.ToString();
            Title += $" (x{Regex.Match(architecture, @"\d.").ToString()})";
        }


        #region Metody Kontrolek
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartupWindow startupWindow = new StartupWindow();
            startupWindow.Show();

            FilesControlObject = new FilesControl();
            SettingsControlObject = new SettingsControl();
            PlayerControlObject = new PlayerControl();
            UserRatingControlObject = new UserRatingControl();

            SV_MainDisplay.Content = FilesControlObject;
            MakeButtonPressedOnLeft(Button_FileControl);
            await WarmupMediaInfoEngine();
            Console.WriteLine(1);
            await SearchForVLCDLL();
            Console.WriteLine(2);
            await LoadUserSettings();
            Console.WriteLine(3);
            startupWindow.Close();

            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Activate();
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
            switch (e.Key)
            {
                case Key.F12:
                    SwitchFullScreen(false);
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Helper.DeleteAllTemporaryFiles();
            Environment.Exit(0);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MakeButtonPressedOnLeft(sender);
            SV_MainDisplay.Content = UserRatingControlObject;
        }
        #endregion
    }
}
