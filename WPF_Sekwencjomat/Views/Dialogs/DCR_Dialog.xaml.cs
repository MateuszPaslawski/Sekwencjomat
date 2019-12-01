using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Sekwencjomat.Views.Dialogs
{
    public partial class DCR_Dialog : Window
    {
        public int Result = 0;

        public DCR_Dialog()
        {
            Owner = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
