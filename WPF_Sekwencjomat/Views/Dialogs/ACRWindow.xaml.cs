using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string Result = null;
        private int counter = Helper.RatingDelay;

        public ACRWindow()
        {
            Owner = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (counter != 0)
                return;

            foreach (var item in UniformGrid_Main.Children)
            {
                if (item is RadioButton && ((RadioButton)item).IsChecked == true)
                    Result = ((RadioButton)item).Content.ToString();
            }
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (counter > 0)
            {
                Label_Counter.Visibility = Visibility.Visible;
                Image_Completed.Visibility = Visibility.Collapsed;

                await Task.Run(() =>
                {
                    
                    while (counter > 0)
                    {

                        Dispatcher.Invoke(() =>
                        {
                            Label_Counter.Content = counter;
                        });

                        Thread.Sleep(1000);

                        --counter;
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
