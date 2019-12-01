using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sekwencjomat.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ACRWindow.xaml
    /// </summary>
    public partial class ACR_Dialog : Window
    {
        public int Result = 0;
        private int Counter = Helper.RatingDelay;

        public ACR_Dialog()
        {
            Owner = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Counter != 0)
            {
                return;
            }

            foreach (object item in UniformGrid_Main.Children)
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
