using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<MediaFile> ListOfMediaFilesWithGrades = new List<MediaFile>();
        private List<MediaFile> ListOfMediaFiles = new List<MediaFile>();
        private Stopwatch TimeLeft = new Stopwatch();
        private int CurrentPlayingFileIndex = 0;


        public void StopPlayer()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Stop();
            });

            Dispatcher.Invoke(() =>
            {
                Helper.ChangeStatusControl($"Odtwarzanie {ListOfMediaFiles.Count} sekwencji zostało zakończone.", false);
                Helper.EnableNavigationButtons();
            });

            CurrentPlayingFileIndex = 0;
            ListOfMediaFiles = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);
        }

        private void MakePlaybackOrder()
        {
            switch (Helper.CurrentPlaybackTechnique)
            {
                case Helper.PlaybackTechnique.ACR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ListOfMediaFiles = Helper.ACR_Descending(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ListOfMediaFiles = Helper.ACR_Ascending(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackMode.Concave:
                            ListOfMediaFiles = Helper.ACR_Concave(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackMode.Convex:
                            ListOfMediaFiles = Helper.ACR_Convex(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackMode.Random:
                            ListOfMediaFiles = Helper.ACR_Random(ListOfMediaFiles);
                            break;
                    }
                    break;

                case Helper.PlaybackTechnique.CCR:
                    throw new NotImplementedException();

                case Helper.PlaybackTechnique.DCR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackMode.Descending:
                            ListOfMediaFiles = Helper.DCR_Descending(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Ascending:
                            ListOfMediaFiles = Helper.DCR_Ascending(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Concave:
                            ListOfMediaFiles = Helper.DCR_Concave(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Convex:
                            ListOfMediaFiles = Helper.DCR_Convex(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackMode.Random:
                            ListOfMediaFiles = Helper.DCR_Random(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                    }
                    break;

                case Helper.PlaybackTechnique.DCRmod:
                    throw new NotImplementedException();
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
                    MediaFile tempFile = ListOfMediaFiles[CurrentPlayingFileIndex - 1] as MediaFile;
                    tempFile.UserGrade = dialog.Result;
                    ListOfMediaFilesWithGrades.Add(tempFile);
                });
            }
            else if (PlayerPlaybackTechnique == Helper.PlaybackTechnique.ACR)
            {
                Dispatcher.Invoke(() =>
                {
                    ACRWindow dialog = new ACRWindow();
                    dialog.ShowDialog();
                    MediaFile tempFile = ListOfMediaFiles[CurrentPlayingFileIndex-1] as MediaFile;
                    tempFile.UserGrade = dialog.Result;
                    ListOfMediaFilesWithGrades.Add(tempFile);
                });
            }



            if (CurrentPlayingFileIndex >= ListOfMediaFiles.Count)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Stop();
                });

                Dispatcher.Invoke(() =>
                {
                    Rating tmpRating = new Rating
                    {
                        RatingTimeSpan = TimeLeft.Elapsed,
                        RatingSeconds = Helper.RatingDelay,
                        ReferenceVideoPath = FilesControlObject.TextBox_RefPath.Text,
                        PlaybackMode = Helper.CurrentPlaybackMode,
                        PlaybackTechnique = Helper.CurrentPlaybackTechnique,
                        FilesListWithGrades = ListOfMediaFilesWithGrades,
                    };

                    Logger.LogRatingToCSV(tmpRating);
                    Helper.EnableNavigationButtons();
                    Helper.ChangeStatusControl($"Odtwarzanie {ListOfMediaFiles.Count} sekwencji zostało zakończone.", false);
                });

                CurrentPlayingFileIndex = 0;
                return;
            }
            else if (CurrentPlayingFileIndex < ListOfMediaFiles.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    Helper.ChangeStatusControl($"Technika: {PlayerPlaybackTechnique.ToString()}\tKolejność: {PlayerPlaybackMode}\tPlik: {ListOfMediaFiles[CurrentPlayingFileIndex].Path}", false);
                });

                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFiles[CurrentPlayingFileIndex].Path));
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

            ListOfMediaFiles = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);

            CurrentPlayingFileIndex = 0;

            MakePlaybackOrder();

            PlayerPlaybackTechnique = Helper.CurrentPlaybackTechnique;
            PlayerPlaybackMode = Helper.CurrentPlaybackMode;

            

            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Audio.Volume = 0;
                VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFiles[0].Path));
            });

            TimeLeft.Start();

            Helper.ChangeStatusControl($"Technika: {PlayerPlaybackTechnique.ToString()}\tKolejność: {PlayerPlaybackMode}\tPlik: {ListOfMediaFiles[0].Path}", false);
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
