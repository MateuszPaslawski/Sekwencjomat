using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF_Sekwencjomat.Models;
using WPF_Sekwencjomat.Views.Dialogs;

namespace WPF_Sekwencjomat
{
    public partial class PlayerControl : UserControl
    {
        public SettingsControl SettingsControlObject = ((MainWindow)Application.Current.MainWindow).SettingsControlObject;
        public FilesControl FilesControlObject = ((MainWindow)Application.Current.MainWindow).FilesControlObject;

        public Helper.PlaybackTechnique PlayerPlaybackTechnique;
        public Helper.PlaybackMode PlayerPlaybackMode;

        private List<MediaFile> ListOfMediaFilePropeties = new List<MediaFile>();
        private int CurrentPlayingFileIndex = 0;
        

        public void StopPlayer()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Stop();
            });

            Dispatcher.Invoke(() =>
            {
                Helper.ChangeStatusControl($"Odtwarzanie {ListOfMediaFilePropeties.Count} sekwencji zostało zakończone.", false);
                Helper.EnableNavigationButtons();
            });

            CurrentPlayingFileIndex = 0;
            ListOfMediaFilePropeties = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);
        }

        private void MakePlaybackOrder()
        {
            switch (Helper.CurrentPlaybackTechnique)
            {
                case Helper.PlaybackTechnique.ACR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ListOfMediaFilePropeties = Helper.ACR_Descending(ListOfMediaFilePropeties);
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ListOfMediaFilePropeties = Helper.ACR_Ascending(ListOfMediaFilePropeties);
                            break;
                        case Helper.PlaybackMode.Concave:
                            ListOfMediaFilePropeties = Helper.ACR_Concave(ListOfMediaFilePropeties);
                            break;
                        case Helper.PlaybackMode.Convex:
                            ListOfMediaFilePropeties = Helper.ACR_Convex(ListOfMediaFilePropeties);
                            break;
                        case Helper.PlaybackMode.Random:
                            ListOfMediaFilePropeties = Helper.ACR_Random(ListOfMediaFilePropeties);
                            break;
                    }
                    break;

                case Helper.PlaybackTechnique.CCR:
                    throw new NotImplementedException();

                case Helper.PlaybackTechnique.DCR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ListOfMediaFilePropeties = Helper.DCR_Descending(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ListOfMediaFilePropeties = Helper.DCR_Ascending(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Concave:
                            ListOfMediaFilePropeties = Helper.DCR_Concave(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Convex:
                            ListOfMediaFilePropeties = Helper.DCR_Convex(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Random:
                            ListOfMediaFilePropeties = Helper.DCR_Random(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text);
                            break;
                    }
                    break;

                case Helper.PlaybackTechnique.DCRmod:
                    throw new NotImplementedException();
            }

            foreach (var item in ListOfMediaFilePropeties)
            {
                Console.WriteLine(item.Path);
            }
        }

        public bool CheckBeforeStartPlaying()
        {
                if (File.Exists(FilesControlObject.TextBox_RefPath.Text))
                    return true;
                else 
                    return false;
        }

        public PlayerControl()
        {
            InitializeComponent();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer != null)
            {
                if (FilesControlObject.ListOfMediaFilePropeties.Count < 1 || CheckBeforeStartPlaying() == false)
                    DP_Navigation.IsEnabled = false;
                else
                {
                    VLC_Control.SourceProvider.MediaPlayer.EndReached -= MediaPlayer_EndReached;
                    VLC_Control.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
                    DP_Navigation.IsEnabled = true;
                }
            }

        }

        private void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            CurrentPlayingFileIndex++;

            if(PlayerPlaybackTechnique == Helper.PlaybackTechnique.DCR && CurrentPlayingFileIndex % 2 == 0)
            {
                Dispatcher.Invoke(() => 
                { 
                    DCRWindow dialog = new DCRWindow();
                    dialog.ShowDialog();
                    Console.WriteLine($"[{ListOfMediaFilePropeties[CurrentPlayingFileIndex-1].Name}]RESULT: {Regex.Match(dialog.Result, "^[1-5]")}");
                });
            }
            else if (PlayerPlaybackTechnique == Helper.PlaybackTechnique.ACR)
            {
                Dispatcher.Invoke(() =>
                {
                    ACRWindow dialog = new ACRWindow();
                    dialog.ShowDialog();
                    Console.WriteLine($"[{ListOfMediaFilePropeties[CurrentPlayingFileIndex-1].Name}]RESULT: {Regex.Match(dialog.Result, "^[1-5]")}");
                });
            }



            if (CurrentPlayingFileIndex >= ListOfMediaFilePropeties.Count)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Stop();
                });

                Dispatcher.Invoke(() =>
                {
                    Helper.EnableNavigationButtons();
                    Helper.ChangeStatusControl($"Odtwarzanie {ListOfMediaFilePropeties.Count} sekwencji zostało zakończone.", false);
                });

                CurrentPlayingFileIndex = 0;
                return;
            }
            else if (CurrentPlayingFileIndex < ListOfMediaFilePropeties.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    Helper.ChangeStatusControl($"Technika: {PlayerPlaybackTechnique.ToString()}\tKolejność: {PlayerPlaybackMode}\tPlik: {ListOfMediaFilePropeties[0].Path}", false);
                });

                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFilePropeties[CurrentPlayingFileIndex].Path));
                });
            }
        }

        private void PLAY_Button_Click(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                MessageBoxResult mb = MessageBox.Show("Rozpocząć odtwarzanie od początku?", "Czy aby napewno?", MessageBoxButton.YesNo);
                if (mb != MessageBoxResult.Yes)
                    return;
            }

            if (FilesControlObject.ListOfMediaFilePropeties == null) { return; }

            ListOfMediaFilePropeties = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);

            CurrentPlayingFileIndex = 0;

            MakePlaybackOrder();

            PlayerPlaybackTechnique = Helper.CurrentPlaybackTechnique;
            PlayerPlaybackMode = Helper.CurrentPlaybackMode;

            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Audio.Volume = 0;
                VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFilePropeties[0].Path));
            });

            Helper.ChangeStatusControl($"Technika: {PlayerPlaybackTechnique.ToString()}\tKolejność: {PlayerPlaybackMode}\tPlik: {ListOfMediaFilePropeties[0].Path}", false);
            Helper.DisableNavigationButtons();
        }

        private void STOP_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                MessageBoxResult mb = MessageBox.Show("Zatrzymać odtwarzanie sekwencji?", "Czy aby napewno?", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.Yes) { StopPlayer(); }
            }
        }

        private void VLC_Control_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SwitchFullScreen(false);
        }
    }
}
