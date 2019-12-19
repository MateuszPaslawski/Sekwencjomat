using Microsoft.Win32;
using Sekwencjomat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

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
                        Logger.TemporaryRatingToTXT(r, fileType);
                        break;
                    case Helper.FileTypeEnum.HTML:
                        Logger.TemporaryRatingToHTML(r, fileType);
                        break;
                    case Helper.FileTypeEnum.CSV:
                        Logger.TemporaryRatingToCSV(r, fileType);
                        break;
                }
            }
        }

        private void RemoveRowFromDataGrid()
        {
            if (DG_Main.SelectedItems.Count > 1)
            {
                MessageBoxResult dialog = MessageBox.Show($"Czy chcesz usunąć wszystkie zaznaczone pliki z tabeli?\nIlość plików: {DG_Main.SelectedItems.Count}", "Otwieranie eksploratora", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            List<object> listToDelete = new List<object>();
            foreach (object item in DG_Main.SelectedItems)
            {
                MediaFile mf = item as MediaFile;
                listToDelete.Add(item);
            }
            foreach (object item in listToDelete)
            {
                DG_Main.Items.Remove(item);
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
            Button bt = sender as Button;
            ContextMenu cm = bt.TemplatedParent as ContextMenu;
            cm.IsOpen = false;
            RemoveRowFromDataGrid();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Rating item = ((DataGridRow)sender).Item as Rating;
        }

        private void Button_Click_Serialize(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog() { Filter = "Plik XML | *.xml", FileName = "WynikiOceny-Sekwencjomat" };
            if (fd.ShowDialog() == true)
                Logger.SerializeToFile(DG_Main.Items.OfType<Rating>().ToList(), fd.FileName);
        }

        private void Button_Click_Deserialize(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Multiselect = false, Filter = "Plik XML | *.xml" };
            if (fd.ShowDialog() != true)
                return;

            DG_Main.Items.Clear();
            foreach (Rating item in Logger.DeserializeFromFile(fd.FileName))
                DG_Main.Items.Add(item);
        }

        private void Button_Click_DownloadPackage(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<Rating> list = DG_Main.Items.OfType<Rating>().ToList();
                Logger.LogRatingToPackage(list, dialog.SelectedPath);
            }
        }
    }
}
