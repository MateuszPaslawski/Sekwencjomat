using Sekwencjomat.Models;
using Sekwencjomat.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sekwencjomat.Controls
{
    public partial class PlayerControl : UserControl
    {
        public SettingsControl SettingsControlObject = ((MainWindow)Application.Current.MainWindow).SettingsControlObject;
        public FilesControl FilesControlObject = ((MainWindow)Application.Current.MainWindow).FilesControlObject;

        public Helper.PlaybackScaleEnum PlayerPlaybackScale;
        public Helper.PlaybackModeEnum PlayerPlaybackMode;

        private readonly List<MediaFile> ListOfMediaFilesWithGrades = new List<MediaFile>();
        private List<MediaFile> ListOfMediaFiles;
        private readonly Stopwatch TimeLeft = new Stopwatch();
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
                case Helper.PlaybackScaleEnum.ACR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackModeEnum.Descending:
                            ListOfMediaFiles = Helper.ACR_Descending(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackModeEnum.Ascending:
                            ListOfMediaFiles = Helper.ACR_Ascending(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackModeEnum.Concave:
                            ListOfMediaFiles = Helper.ACR_Concave(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackModeEnum.Convex:
                            ListOfMediaFiles = Helper.ACR_Convex(ListOfMediaFiles);
                            break;
                        case Helper.PlaybackModeEnum.Random:
                            ListOfMediaFiles = Helper.ACR_Random(ListOfMediaFiles);
                            break;
                    }
                    break;

                case Helper.PlaybackScaleEnum.CCR:
                    throw new NotImplementedException();

                case Helper.PlaybackScaleEnum.DCR:
                    switch (Helper.CurrentPlaybackMode)
                    {
                        case Helper.PlaybackModeEnum.Descending:
                            ListOfMediaFiles = Helper.DCR_Descending(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackModeEnum.Ascending:
                            ListOfMediaFiles = Helper.DCR_Ascending(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackModeEnum.Concave:
                            ListOfMediaFiles = Helper.DCR_Concave(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackModeEnum.Convex:
                            ListOfMediaFiles = Helper.DCR_Convex(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                        case Helper.PlaybackModeEnum.Random:
                            ListOfMediaFiles = Helper.DCR_Random(ListOfMediaFiles, FilesControlObject.TextBox_RefPath.Text);
                            break;
                    }
                    break;

                case Helper.PlaybackScaleEnum.DCRmod:
                    throw new NotImplementedException();
            }
        }

        public bool CheckBeforeStartPlaying()
        {
            try
            {
                VLC_Control.SourceProvider.MediaPlayer.EndReached -= MediaPlayer_EndReached;
                VLC_Control.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
            }
            catch
            {
                MessageBox.Show("Wystąpił błąd uruchamiania odtwarzacza. Upewnij się, że wersja aplikacji odpowiada wersji programu VLC (32bit / 64bit).", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Helper.CurrentPlaybackScale == Helper.PlaybackScaleEnum.ACR)
            {
                return true;
            }

            if (File.Exists(FilesControlObject.TextBox_RefPath.Text))
            {
                return true;
            }
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
            if (FilesControlObject.ListOfMediaFilsInGrid.Count < 1 || CheckBeforeStartPlaying() == false)
            {
                DP_Navigation.IsEnabled = false;
            }
            else
            {
                VLC_Control.SourceProvider.MediaPlayer.EndReached -= MediaPlayer_EndReached;
                VLC_Control.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
                DP_Navigation.IsEnabled = true;
            }

        }

        private void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            CurrentPlayingFileIndex++;

            if (PlayerPlaybackScale == Helper.PlaybackScaleEnum.DCR && CurrentPlayingFileIndex % 2 == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    DCR_Dialog dialog = new DCR_Dialog();
                    dialog.ShowDialog();
                    MediaFile tempFile = ListOfMediaFiles[CurrentPlayingFileIndex - 1] as MediaFile;
                    tempFile.UserGrade = dialog.Result;
                    ListOfMediaFilesWithGrades.Add(tempFile);
                });
            }
            else if (PlayerPlaybackScale == Helper.PlaybackScaleEnum.ACR)
            {
                Dispatcher.Invoke(() =>
                {
                    ACR_Dialog dialog = new ACR_Dialog();
                    dialog.ShowDialog();
                    MediaFile tempFile = ListOfMediaFiles[CurrentPlayingFileIndex - 1] as MediaFile;
                    tempFile.UserGrade = dialog.Result;
                    ListOfMediaFilesWithGrades.Add(tempFile);
                    Console.WriteLine($"\n\nAdding {tempFile.Path}   Count: {ListOfMediaFilesWithGrades.Count}");
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
                        DurationSeconds = (int)TimeLeft.Elapsed.TotalSeconds,
                        RatingSeconds = Helper.RatingDelay,
                        ReferenceVideoPath = FilesControlObject.TextBox_RefPath.Text,
                        PlaybackMode = Helper.CurrentPlaybackMode,
                        PlaybackScale = Helper.CurrentPlaybackScale,
                        FilesListWithGrades = new List<MediaFile>(ListOfMediaFilesWithGrades),
                        DateTimeRating = DateTime.Now,
                    };

                    Helper.AddUserRatingToGrid(tmpRating);
                    ListOfMediaFilesWithGrades.Clear();
                    Helper.EnableNavigationButtons();
                    Helper.ResetStatusControl();
                });

                CurrentPlayingFileIndex = 0;
                return;
            }
            else if (CurrentPlayingFileIndex < ListOfMediaFiles.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    Helper.ChangeStatusControl($"Metoda MOS: {PlayerPlaybackScale.ToString()} | Kolejność: {Helper.PlaybackModeToString(PlayerPlaybackMode)} | Plik: {ListOfMediaFiles[CurrentPlayingFileIndex].Path}", false);
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
                MessageBoxResult mb = MessageBox.Show("Rozpocząć odtwarzanie od początku?", "", MessageBoxButton.YesNo);

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
            Helper.ChangeStatusControl($"Metoda MOS: {PlayerPlaybackScale.ToString()} | Kolejność: {Helper.PlaybackModeToString(PlayerPlaybackMode)} | Plik: {ListOfMediaFiles[CurrentPlayingFileIndex].Path}", false);
            Helper.DisableNavigationButtons();
        }

        private void STOP_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                PausePlayer();
                MessageBoxResult mb = MessageBox.Show("Zatrzymać odtwarzanie sekwencji?", "", MessageBoxButton.YesNo);

                if (mb == MessageBoxResult.Yes)
                {
                    StopPlayer();
                }
                else
                {
                    UnpausePlayer();
                }
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
