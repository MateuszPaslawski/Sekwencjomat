using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Sekwencjomat.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ACRWindow.xaml
    /// </summary>
    public partial class ACRWindow : Window
    {
        public string Result = null;

        public ACRWindow()
        {
            Owner = (MainWindow)App.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in UniformGrid_Main.Children)
            {
                if (item is RadioButton && ((RadioButton)item).IsChecked == true)
                    Result = ((RadioButton)item).Content.ToString();
            }
            Close();
        }
    }
}
