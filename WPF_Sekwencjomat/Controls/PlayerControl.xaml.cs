using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF_Sekwencjomat.Models;

namespace WPF_Sekwencjomat
{
    public partial class PlayerControl : UserControl
    {
        SettingsControl SettingsControlObject = ((MainWindow)Application.Current.MainWindow).SettingsControlObject;
        private readonly FilesControl FilesControlObject = ((MainWindow)Application.Current.MainWindow).FilesControlObject;
        private readonly List<Button> ListOfButtons = new List<Button>();
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
            });

            CurrentPlayingFileIndex = 0;
            ListOfMediaFilePropeties = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);
            MakePlaybackOrder();
        }

        public bool CheckBeforeStartPlaying()
        {
            if (Helper.GetPlaybackOrder() == "sequentialforrating")
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
            switch (Helper.GetPlaybackOrder())
            {
                case "random":
                    ListOfMediaFilePropeties = Helper.ShuffleList(ListOfMediaFilePropeties);
                    break;
                case "bitrateascending":
                    ListOfMediaFilePropeties = Helper.AscendingBitrate(ListOfMediaFilePropeties);
                    break;
                case "bitratedescending":
                    ListOfMediaFilePropeties = Helper.DescendingBitrate(ListOfMediaFilePropeties);
                    break;
                case "resolutionascending":
                    ListOfMediaFilePropeties = Helper.AscendingResolution(ListOfMediaFilePropeties);
                    break;
                case "resolutiondescending":
                    ListOfMediaFilePropeties = Helper.DescendingResolution(ListOfMediaFilePropeties);
                    break;
                case "sequentialforrating":
                    ListOfMediaFilePropeties = Helper.SequentialWithRefFile(ListOfMediaFilePropeties, FilesControlObject.TextBox_RefPath.Text, FilesControlObject.TextBox_PauserPath.Text);
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
            Console.WriteLine(Helper.CurrentPlaybackMode);
            Console.WriteLine(Helper.CurrentPlaybackTechnique);
            if (VLC_Control.SourceProvider.MediaPlayer != null)
            {
                if (FilesControlObject.ListOfMediaFilePropeties.Count < 1 || CheckBeforeStartPlaying() == false)
                {
                    DisableAllNavigation();
                }
                else
                {
                    ListOfMediaFilePropeties = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);
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

            if (CurrentPlayingFileIndex >= ListOfMediaFilePropeties.Count)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    VLC_Control.SourceProvider.MediaPlayer.Stop();

                });

                Dispatcher.Invoke(() =>
                {
                    Helper.ChangeStatusControl($"Odtwarzanie {ListOfMediaFilePropeties.Count} sekwencji zostało zakończone.", false);
                });

                CurrentPlayingFileIndex = 0;
                return;
            }
            else if (CurrentPlayingFileIndex < ListOfMediaFilePropeties.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    Helper.ChangeStatusControl($"Odtwarzanie pliku: {ListOfMediaFilePropeties[CurrentPlayingFileIndex].Path}", false);
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
                {
                    return;
                }
            }

            ListOfMediaFilePropeties = new List<MediaFile>(FilesControlObject.ListOfMediaFilePropeties);
            MakePlaybackOrder();
            if (ListOfMediaFilePropeties == null) { return; }
            CurrentPlayingFileIndex = 0;

            ThreadPool.QueueUserWorkItem(x =>
            {
                VLC_Control.SourceProvider.MediaPlayer.Audio.Volume = 20;
                VLC_Control.SourceProvider.MediaPlayer.Play(new Uri(ListOfMediaFilePropeties[0].Path));
            });

            Helper.ChangeStatusControl($"Odtwarzanie pliku: {ListOfMediaFilePropeties[0].Path}", false);
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

        private void VLC_Control_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SwitchFullScreen(false);
        }
    }
}
