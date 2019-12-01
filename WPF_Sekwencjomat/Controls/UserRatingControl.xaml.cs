using Microsoft.Win32;
using Sekwencjomat.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        private void SerializeToFile(string fileName)
        {
            List<Rating> list = new List<Rating>();

            foreach (Rating item in DG_Main.Items)
                list.Add(item);

            Type[] Types = { typeof(MediaFile), typeof(Rating) };
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rating>), Types);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, list);
            fs.Close();
        }

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
            Console.WriteLine(item.DateTimeRatingString);
        }

        private void Button_Click_Serialize(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog() { Filter = "Plik XML | *.xml", FileName = "WynikiOceny-Sekwencjomat" };
            if (fd.ShowDialog() == true)
                SerializeToFile(fd.FileName);
        }

        private void Button_Click_Deserialize(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() {Multiselect = false, Filter = "Plik XML | *.xml" };
            if (fd.ShowDialog() != true)
                return;

            Type[] Types = { typeof(MediaFile), typeof(Rating) };
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rating>), Types);
            FileStream fs = new FileStream(fd.FileName, FileMode.Open);
            List<Rating> list = (List<Rating>)serializer.Deserialize(fs);
            serializer.Serialize(Stream.Null, list);

            foreach (Rating item in list)
                DG_Main.Items.Add(item);
        }

        private void Button_Click_DownloadPackage(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }
    }
}
