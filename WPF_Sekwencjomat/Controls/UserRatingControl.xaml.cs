using Sekwencjomat.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sekwencjomat.Controls
{
    /// <summary>
    /// Interaction logic for UserRatingControl.xaml
    /// </summary>
    public partial class UserRatingControl : UserControl
    {
        public void OpenFileFromDataGrid(Helper.FileTypeEnum fileType)
        {
            if (DG_Main.SelectedItems.Count > 1)
            {
                MessageBoxResult dialog = MessageBox.Show($"Czy chcesz odtworzyć wszystkie zaznaczone pliki?\nIlość plików: {DG_Main.SelectedItems.Count}", "Otwieranie eksploratora", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            foreach (object item in DG_Main.SelectedItems)
            {
                Rating r = item as Rating;

                switch (fileType)
                {
                    case Helper.FileTypeEnum.TXT:
                        Logger.LogRatingToTXT(r, fileType);
                        break;
                    case Helper.FileTypeEnum.HTML:
                        Logger.LogRatingToHTML(r, fileType);
                        break;
                    case Helper.FileTypeEnum.CSV:
                        Logger.LogRatingToCSV(r, fileType);
                        break;
                }
            }
        }

        public UserRatingControl()
        {
            InitializeComponent();
        }

        private void DG_Main_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void MenuItem_Click_TXT(object sender, RoutedEventArgs e)
        {
            OpenFileFromDataGrid(Helper.FileTypeEnum.TXT);
        }

        private void MenuItem_Click_CSV(object sender, RoutedEventArgs e)
        {
            OpenFileFromDataGrid(Helper.FileTypeEnum.CSV);
        }

        private void MenuItem_Click_HTML(object sender, RoutedEventArgs e)
        {
            OpenFileFromDataGrid(Helper.FileTypeEnum.HTML);

        }

        private void MenuItem_Click_DeleteRow(object sender, RoutedEventArgs e)
        {

        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Rating item = ((DataGridRow)sender).Item as Rating;
            Console.WriteLine(item.DateTimeRatingString);
        }
    }
}
