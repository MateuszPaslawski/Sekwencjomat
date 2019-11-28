using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Sekwencjomat.Models;
using Sekwencjomat.Views.Dialogs;

namespace Sekwencjomat
{
    public partial class PlayerControl : UserControl
    {
        public SettingsControl SettingsControlObject = ((MainWindow)Application.Current.MainWindow).SettingsControlObject;
        public FilesControl FilesControlObject = ((MainWindow)Application.Current.MainWindow).FilesControlObject;

        public Helper.PlaybackScale PlayerPlaybackScale;
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
                Helper.ResetStatusControl();
                Helper.EnableNavigationButtons();
            });

            CurrentPlayingFileIndex = 0;
            ListOfMediaFiles = new List<MediaFile>(FilesControlObject.ListOfMediaFilsInGrid);
        }

        public void PausePlayer()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Pause();
            });
        }

        public void UnpausePlayer()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Play();
            });
        }

        private void MakePlaybackOrder()
        {
            switch (Helper.CurrentPlaybackScale)
            {
                case Helper.PlaybackScale.ACR:
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

                case Helper.PlaybackScale.CCR:
                    throw new NotImplementedException();

                case Helper.PlaybackScale.DCR:
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

                case Helper.PlaybackScale.DCRmod:
                    throw new NotImplementedException();
            }
        }

        public bool CheckBeforeStartPlaying()
        {
            if (Helper.CurrentPlaybackScale == Helper.PlaybackScale.ACR)
                return true;

            if (File.Exists(FilesControlObject.TextBox_RefPath.Text))
                return true;
            else
            {
                FilesControlObject.TextBox_RefPath.Text = string.Empty;
                return false;
            }
        }



        public PlayerControl()
        {
            InitializeComponent();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer != null)
            {
                if (FilesControlObject.ListOfMediaFilsInGrid.Count < 1 || CheckBeforeStartPlaying() == false)
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

            if(PlayerPlaybackScale == Helper.PlaybackScale.DCR && CurrentPlayingFileIndex % 2 == 0)
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
            else if (PlayerPlaybackScale == Helper.PlaybackScale.ACR)
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
                        PlaybackScale = Helper.CurrentPlaybackScale,
                        FilesListWithGrades = ListOfMediaFilesWithGrades,
                    };

                    Logger.LogRatingToCSV(tmpRating);
                    Logger.LogRatingToTXT(tmpRating);
                    Logger.LogRatingToHTML(tmpRating);
                    ListOfMediaFilesWithGrades.Clear();
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
                    Helper.ChangeStatusControl($"Metoda MOS: {PlayerPlaybackScale.ToString()} | Kolejność: {PlayerPlaybackMode} | Plik: {ListOfMediaFiles[CurrentPlayingFileIndex].Path}", false);
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
                PausePlayer();
                MessageBoxResult mb = MessageBox.Show("Rozpocząć odtwarzanie od początku?", "Czy aby napewno?", MessageBoxButton.YesNo);

                if (mb != MessageBoxResult.Yes)
                {
                    UnpausePlayer();
                    return;
                }
            }

            if (FilesControlObject.ListOfMediaFilsInGrid.Count == 0) { return; }

            ListOfMediaFiles = new List<MediaFile>(FilesControlObject.ListOfMediaFilsInGrid);

            CurrentPlayingFileIndex = 0;

            MakePlaybackOrder();

            PlayerPlaybackScale = Helper.CurrentPlaybackScale;
            PlayerPlaybackMode = Helper.CurrentPlaybackMode;

            

            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Audio.Volume = 0;
                VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFiles[0].Path));
            });

            TimeLeft.Restart();
            Helper.ChangeStatusControl($"Metoda MOS: {PlayerPlaybackScale.ToString()} | Kolejność: {PlayerPlaybackMode} | Plik: {ListOfMediaFiles[CurrentPlayingFileIndex].Path}", false);
            Helper.DisableNavigationButtons();
        }

        private void STOP_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                PausePlayer();
                MessageBoxResult mb = MessageBox.Show("Zatrzymać odtwarzanie sekwencji?", "Czy aby napewno?", MessageBoxButton.YesNo);

                if (mb == MessageBoxResult.Yes)
                    StopPlayer();
                else
                    UnpausePlayer();
            }
        }

        private void PlayerControl_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SwitchFullScreen(false);
        }

        private void FullScreen_Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SwitchFullScreen(false);
        }
    }
}
