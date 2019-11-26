using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class DCRWindow : Window
    {
        public int Result = 0;

        public DCRWindow()
        {
            Owner = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in UniformGrid_Main.Children)
            {
                if (item is RadioButton && ((RadioButton)item).IsChecked == true)
                {
                    string regexValue = Regex.Match(((RadioButton)item).Content.ToString(), @"\d").Value.ToString();
                    Result = int.Parse(regexValue);
                }
            }
            Close();
        }
    }
}
