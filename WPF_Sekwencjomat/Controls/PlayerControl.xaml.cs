using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPF_Sekwencjomat
{
    public partial class PlayerControl : UserControl
    {
        //SettingsControl SettingsControlObject = ((MainWindow)Application.Current.MainWindow).SettingsControlObject;
        private readonly FilesControl FilesControlObject = ((MainWindow)Application.Current.MainWindow).FilesControlObject;
        private readonly List<Button> ListOfButtons = new List<Button>();
        private List<FileClass> ListOfFileClass = new List<FileClass>();
        private int CurrentPlayingFileIndex = 0;

        public void StopPlayer()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Stop();

            });

            Dispatcher.Invoke(() =>
            {
                Utils.SetCurrentInfo($"Odtwarzanie {ListOfFileClass.Count} sekwencji zostało zakończone.", false);
            });

            CurrentPlayingFileIndex = 0;
            ListOfFileClass = new List<FileClass>(FilesControlObject.ListOfFileClass);
            MakePlaybackOrder();
        }

        public bool CheckBeforeStartPlaying()
        {
            if (Utils.GetPlaybackOrder() == "sequentialforrating")
            {
                if (File.Exists(FilesControlObject.TextBox_PauserPath.Text)
                    && File.Exists(FilesControlObject.TextBox_RefPath.Text))
                {
                    return true;
                }
                else { return false; }
            }
            else
            {
                return true;
            }
        }

        private void FillListOfButtons()
        {
            foreach (object item in DP_Navigation.Children)
            {
                if (!(item is Button)) { continue; }

                Button btn = item as Button;

                ListOfButtons.Add(btn);
            }
        }

        private void EnableAllNavigation()
        {
            foreach (Button item in ListOfButtons)
            {
                item.IsEnabled = true;
            }
        }

        private void DisableAllNavigation()
        {
            foreach (Button item in ListOfButtons)
            {
                item.IsEnabled = false;
            }
        }

        private void MakePlaybackOrder()
        {
            switch (Utils.GetPlaybackOrder())
            {
                case "random":
                    ListOfFileClass = Utils.ShuffleList(ListOfFileClass);
                    break;
                case "bitrateascending":
                    ListOfFileClass = Utils.FileClassListAscendingBitrate(ListOfFileClass);
                    break;
                case "bitratedescending":
                    ListOfFileClass = Utils.FileClassListDescendingBitrate(ListOfFileClass);
                    break;
                case "resolutionascending":
                    ListOfFileClass = Utils.FileClassListAscendingResolution(ListOfFileClass);
                    break;
                case "resolutiondescending":
                    ListOfFileClass = Utils.FileClassListDescendingResolution(ListOfFileClass);
                    break;
                case "sequentialforrating":
                    ListOfFileClass = Utils.FileClassSequentialForRating(ListOfFileClass, FilesControlObject.TextBox_RefPath.Text, FilesControlObject.TextBox_PauserPath.Text);
                    break;
            }
        }


        public PlayerControl()
        {
            InitializeComponent();
            FillListOfButtons();
            DisableAllNavigation();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer != null)
            {
                if (FilesControlObject.ListOfFileClass.Count < 1 || CheckBeforeStartPlaying() == false)
                {
                    DisableAllNavigation();
                }
                else
                {
                    ListOfFileClass = new List<FileClass>(FilesControlObject.ListOfFileClass);
                    MakePlaybackOrder();

                    VLC_Control.SourceProvider.MediaPlayer.EndReached -= MediaPlayer_EndReached;
                    VLC_Control.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;

                    EnableAllNavigation();
                }
            }
        }

        private void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            CurrentPlayingFileIndex++;

            if (CurrentPlayingFileIndex >= ListOfFileClass.Count)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Stop();

                });

                Dispatcher.Invoke(() =>
                {
                    Utils.SetCurrentInfo($"Odtwarzanie {ListOfFileClass.Count} sekwencji zostało zakończone.", false);
                });

                CurrentPlayingFileIndex = 0;
                return;
            }
            else if (CurrentPlayingFileIndex < ListOfFileClass.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    Utils.SetCurrentInfo($"Odtwarzanie pliku: {ListOfFileClass[CurrentPlayingFileIndex].FilePath}", false);
                });

                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfFileClass[CurrentPlayingFileIndex].FilePath));
                });
            }
        }

        private void PLAY_Button_Click(object sender, RoutedEventArgs e)
        {

            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                MessageBoxResult mb = MessageBox.Show("Rozpocząć odtwarzanie od początku?", "Czy aby napewno?", MessageBoxButton.YesNo);
                if (mb != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            ListOfFileClass = new List<FileClass>(FilesControlObject.ListOfFileClass);
            MakePlaybackOrder();
            if (ListOfFileClass == null) { return; }
            CurrentPlayingFileIndex = 0;

            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Audio.Volume = 20;
                VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfFileClass[0].FilePath));
            });

            Utils.SetCurrentInfo($"Odtwarzanie pliku: {ListOfFileClass[0].FilePath}", false);
        }

        private void PAUSE_Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Pause();
                });
            }
            else
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Play();
                });
            }
        }

        private void STOP_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VLC_Control.SourceProvider.MediaPlayer.IsPlaying())
            {
                MessageBoxResult mb = MessageBox.Show("Zatrzymać odtwarzanie sekwencji?", "Czy aby napewno?", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.Yes) { StopPlayer(); }
            }
        }
    }
}
