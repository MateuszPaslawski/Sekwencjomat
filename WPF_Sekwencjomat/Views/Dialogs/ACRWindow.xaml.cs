using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Sekwencjomat.Models;

namespace WPF_Sekwencjomat.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ACRWindow.xaml
    /// </summary>
    public partial class ACRWindow : Window
    {
        public int Result = 0;
        private int Counter = Helper.RatingDelay;

        public ACRWindow()
        {
            Owner = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Counter != 0)
                return;

            foreach (var item in UniformGrid_Main.Children)
            {
                if (item is RadioButton && ((RadioButton)item).IsChecked == true)
                {
                    string regexValue = Regex.Match(((RadioButton)item).Tag.ToString(), @"\d").Value.ToString();
                    Result = int.Parse(regexValue);
                }
            }
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Counter > 0)
            {
                Label_Counter.Visibility = Visibility.Visible;
                Image_Completed.Visibility = Visibility.Collapsed;

                await Task.Run(() =>
                {
                    
                    while (Counter > 0)
                    {

                        Dispatcher.Invoke(() =>
                        {
                            Label_Counter.Content = Counter;
                        });

                        Thread.Sleep(1000);

                        --Counter;
                    }

                    Dispatcher.Invoke(() => 
                    {
                        Label_Counter.Visibility = Visibility.Collapsed;
                        Image_Completed.Visibility = Visibility.Visible;
                    });
                });

            }
        }
    }
}
