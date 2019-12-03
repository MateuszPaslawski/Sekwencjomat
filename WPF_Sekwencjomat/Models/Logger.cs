using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace Sekwencjomat.Models
{
    public static class Logger
    {
        private static string nowDate
        {
            get
            {
                return DateTime.Now.ToString(@"dd-MM-yyyy");
            }
        }


        public static void SerializeToFile(List<Rating> listRating, string fileName)
        {
            Type[] Types = { typeof(MediaFile), typeof(Rating) };
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rating>), Types);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, listRating);
            fs.Close();
        }

        public static List<Rating> DeserializeFromFile(string fileName)
        {
            try
            {
                Type[] Types = { typeof(MediaFile), typeof(Rating) };
                XmlSerializer serializer = new XmlSerializer(typeof(List<Rating>), Types);
                FileStream fs = new FileStream(fileName, FileMode.Open);
                List<Rating> list = (List<Rating>)serializer.Deserialize(fs);
                serializer.Serialize(Stream.Null, list);
                return list;

            }
            catch (Exception exc)
            {
                MessageBox.Show($"Błąd wczytywania pliku XML\n\n{exc.Message}", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Rating>();
            }

        }



        public static void TemporaryRatingToTXT(Rating rating, Helper.FileTypeEnum fileType)
        {
            string path = Helper.GetTemporaryFilePath(fileType);

            int lp_counter = 1;

            using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
            {
                file.WriteLine($"{"Data:",-20}{rating.DateTimeRatingString}");
                file.WriteLine($"{"Metoda MOS:",-20}{rating.PlaybackScale}");
                file.WriteLine($"{"Kolejność:",-20}{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                file.WriteLine($"{"Czas trwania:",-20}{rating.DurationSecondsString}");
                file.WriteLine($"{"Czas na ocenę [s]:",-20}{rating.RatingSeconds}");
                file.WriteLine();

                if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                {
                    file.WriteLine($"{"Plik referencyjny:",-20}{rating.ReferenceVideoPath}");
                }

                file.WriteLine($"{"Lp.",5} | {"Ocena [1-5]",5} | {"Bitrate [kB/s]",15} | {"Rozdzielczość",15} | {"FPS",5} | {"Rozmiar",10} | {"Ścieżka pliku",-10}");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine($"{lp_counter,5} | {mf.UserGrade,5} | {mf.Bitrate,15} | {mf.FrameSize,15} | {mf.FPS,5} | {mf.Size,10} | {mf.Path,-10}");
                    lp_counter++;
                }
            }
            Process fileProc = new Process();
            fileProc.EnableRaisingEvents = true;
            fileProc.StartInfo.FileName = path;
            fileProc.Start();


        }

        public static void TemporaryRatingToCSV(Rating rating, Helper.FileTypeEnum fileType)
        {
            string path = Helper.GetTemporaryFilePath(fileType);

            int lp_counter = 1;

            using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
            {
                file.WriteLine($"Data;{rating.DateTimeRatingString}");
                file.WriteLine($"Metoda MOS;{rating.PlaybackScale}");
                file.WriteLine($"Kolejność;{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                file.WriteLine($"Czas trwania badania;{rating.DurationSecondsString}");
                file.WriteLine($"Czas na ocenę [s];{rating.RatingSeconds}");
                file.WriteLine();
                if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                {
                    file.WriteLine($"Plik referencyjny;;{rating.ReferenceVideoPath}");
                }

                file.WriteLine($"lp;Ocena [1-5];Bitrate [kB/s];Rozdzielczosc;FPS;Rozmiar;Ścieżka Pliku");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine($"{lp_counter};{mf.UserGrade};{mf.Bitrate};{mf.FrameSize};{mf.FPS};{mf.Size};{mf.Path}");
                    lp_counter++;
                }
            }
            Process fileProc = new Process();
            fileProc.EnableRaisingEvents = true;
            fileProc.StartInfo.FileName = path;
            fileProc.Start();
        }

        public static void TemporaryRatingToHTML(Rating rating, Helper.FileTypeEnum fileType)
        {
            string path = Helper.GetTemporaryFilePath(fileType);

            int lp_counter = 1;

            using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
            {
                file.WriteLine("<html>");
                file.WriteLine("<head>");
                file.WriteLine("<style>");
                file.WriteLine("table, th, td { border: 1px solid black; }");
                file.WriteLine("th, td { padding: 10px; text-align:center }");
                file.WriteLine("tr:nth-child(even) {background-color: #f2f2f2;}");
                file.WriteLine("body {font: normal 14px Verdana}");
                file.WriteLine("</style>");
                file.WriteLine("</head>");


                file.WriteLine("<body>");
                file.WriteLine("<h1><center>Wyniki pomiarów : Sekwencjomat</center></h1>");

                file.WriteLine($"<p>Data: <b>{rating.DateTimeRatingString}</b></p>");
                file.WriteLine($"<p>Metoda MOS: <b>{rating.PlaybackScale}</b></p>");
                file.WriteLine($"<p>Kolejność: <b>{Helper.PlaybackModeToString(rating.PlaybackMode)}</b></p>");
                file.WriteLine($"<p>Czas trwania: <b>{rating.DurationSecondsString}</b></p>");
                file.WriteLine($"<p>Czas na ocenę [s]: <b>{rating.RatingSeconds}</b></p>");

                if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                {
                    file.WriteLine($"<p>Plik referencyjny: <b>{rating.ReferenceVideoPath}</b></p>");
                }

                file.WriteLine("<table style=\"width: 100 %\">");
                file.WriteLine("<tr>");
                file.WriteLine("<th>lp</th>");
                file.WriteLine("<th>Ocena [1-5]</th>");
                file.WriteLine("<th>Bitrate [kB/s]</th>");
                file.WriteLine("<th>Rozdzielczosc</th>");
                file.WriteLine("<th>FPS</th>");
                file.WriteLine("<th>Rozmiar</th>");
                file.WriteLine("<th>Ścieżka Pliku</th>");
                file.WriteLine("</tr>");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine("<tr>");
                    file.WriteLine($"<td>{lp_counter}</td>");
                    file.WriteLine($"<td>{mf.UserGrade}</td>");
                    file.WriteLine($"<td>{mf.Bitrate}</td>");
                    file.WriteLine($"<td>{mf.FrameSize}</td>");
                    file.WriteLine($"<td>{mf.FPS}</td>");
                    file.WriteLine($"<td>{mf.Size}</td>");
                    file.WriteLine($"<td>{mf.Path}</td>");
                    file.WriteLine("</tr>");
                    lp_counter++;
                }
                file.WriteLine("</table>");
                file.WriteLine("</body>");
                file.WriteLine("</html>");
            }

            Process fileProc = new Process();
            fileProc.EnableRaisingEvents = true;
            fileProc.StartInfo.FileName = path;
            fileProc.Start();
        }



        public static void LogRatingToFile(Rating rating, Helper.FileTypeEnum fileType, string saveDirectory)
        {
            string file_name = $"{rating.PlaybackScale}.{fileType.ToString().ToLower()}";
            string path = Path.Combine(saveDirectory, file_name);

            Directory.CreateDirectory(saveDirectory);

            int lp_counter = 1;
            switch (fileType)
            {
                case Helper.FileTypeEnum.TXT:
                    using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        file.WriteLine($"{"Data:",-20}{rating.DateTimeRatingString}");
                        file.WriteLine($"{"Metoda MOS:",-20}{rating.PlaybackScale}");
                        file.WriteLine($"{"Kolejność:",-20}{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                        file.WriteLine($"{"Czas trwania:",-20}{rating.DurationSecondsString}");
                        file.WriteLine($"{"Czas na ocenę [s]:",-20}{rating.RatingSeconds}");

                        if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                        {
                            file.WriteLine($"{"Plik referencyjny:",-20}{rating.ReferenceVideoPath}");
                        }

                        file.WriteLine();
                        file.WriteLine($"{"Lp.",5} | {"Ocena [1-5]",5} | {"Bitrate [kB/s]",15} | {"Rozdzielczość",15} | {"FPS",5} | {"Rozmiar",10} | {"Ścieżka pliku",-10}");

                        foreach (MediaFile mf in rating.FilesListWithGrades)
                        {
                            file.WriteLine($"{lp_counter,5} | {mf.UserGrade,5} | {mf.Bitrate,15} | {mf.FrameSize,15} | {mf.FPS,5} | {mf.Size,10} | {mf.Path,-10}");
                            lp_counter++;
                        }
                    }
                    break;
                case Helper.FileTypeEnum.HTML:
                    using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        file.WriteLine("<html>");
                        file.WriteLine("<head>");
                        file.WriteLine("<style>");
                        file.WriteLine("table, th, td { border: 1px solid black; }");
                        file.WriteLine("th, td { padding: 10px; text-align:center }");
                        file.WriteLine("tr:nth-child(even) {background-color: #f2f2f2;}");
                        file.WriteLine("body {font: normal 14px Verdana}");
                        file.WriteLine("</style>");
                        file.WriteLine("</head>");


                        file.WriteLine("<body>");
                        file.WriteLine("<h1><center>Wyniki pomiarów : Sekwencjomat</center></h1>");

                        file.WriteLine($"<p>Data: <b>{rating.DateTimeRatingString}</b></p>");
                        file.WriteLine($"<p>Metoda MOS: <b>{rating.PlaybackScale}</b></p>");
                        file.WriteLine($"<p>Kolejność: <b>{Helper.PlaybackModeToString(rating.PlaybackMode)}</b></p>");
                        file.WriteLine($"<p>Czas trwania: <b>{rating.DurationSecondsString}</b></p>");
                        file.WriteLine($"<p>Czas na ocenę [s]: <b>{rating.RatingSeconds}</b></p>");

                        if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                        {
                            file.WriteLine($"<p>Plik referencyjny: <b>{rating.ReferenceVideoPath}</b></p>");
                        }

                        file.WriteLine("<table style=\"width: 100 %\">");
                        file.WriteLine("<tr>");
                        file.WriteLine("<th>lp</th>");
                        file.WriteLine("<th>Ocena [1-5]</th>");
                        file.WriteLine("<th>Bitrate [kB/s]</th>");
                        file.WriteLine("<th>Rozdzielczosc</th>");
                        file.WriteLine("<th>FPS</th>");
                        file.WriteLine("<th>Rozmiar</th>");
                        file.WriteLine("<th>Ścieżka Pliku</th>");
                        file.WriteLine("</tr>");

                        foreach (MediaFile mf in rating.FilesListWithGrades)
                        {
                            file.WriteLine("<tr>");
                            file.WriteLine($"<td>{lp_counter}</td>");
                            file.WriteLine($"<td>{mf.UserGrade}</td>");
                            file.WriteLine($"<td>{mf.Bitrate}</td>");
                            file.WriteLine($"<td>{mf.FrameSize}</td>");
                            file.WriteLine($"<td>{mf.FPS}</td>");
                            file.WriteLine($"<td>{mf.Size}</td>");
                            file.WriteLine($"<td>{mf.Path}</td>");
                            file.WriteLine("</tr>");
                            lp_counter++;
                        }
                        file.WriteLine("</table>");
                        file.WriteLine("</body>");
                        file.WriteLine("</html>");
                    }
                    break;
                case Helper.FileTypeEnum.CSV:
                    using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        file.WriteLine($"Data;{rating.DateTimeRatingString}");
                        file.WriteLine($"Metoda MOS;{rating.PlaybackScale}");
                        file.WriteLine($"Kolejność;{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                        file.WriteLine($"Czas trwania badania;{rating.DurationSecondsString}");
                        file.WriteLine($"Czas na ocenę [s];{rating.RatingSeconds}");
                        file.WriteLine();
                        if (rating.PlaybackScale == Helper.PlaybackScaleEnum.DCR)
                        {
                            file.WriteLine($"Plik referencyjny;;{rating.ReferenceVideoPath}");
                        }

                        file.WriteLine($"lp;Ocena [1-5];Bitrate [kB/s];Rozdzielczosc;FPS;Rozmiar;Ścieżka Pliku");

                        foreach (MediaFile mf in rating.FilesListWithGrades)
                        {
                            file.WriteLine($"{lp_counter};{mf.UserGrade};{mf.Bitrate};{mf.FrameSize};{mf.FPS};{mf.Size};{mf.Path}");
                            lp_counter++;
                        }
                    }
                    break;
            }
        }

        public static void LogRatingToPackage(List<Rating> ratingList, string rootDirectory)
        {
            rootDirectory = Path.Combine(rootDirectory, $"{nowDate}-Sekwencjomat");

            string testDirectory = rootDirectory;
            int dir_incerement = 1;
            int packages_couter = 0;

            while (Directory.Exists(testDirectory))
            {
                testDirectory =  $"{rootDirectory} ({dir_incerement})";
                dir_incerement++;
            }

            rootDirectory = testDirectory;


            foreach (Rating rating_item in ratingList)
            {
                packages_couter++;
                string package_directory = Path.Combine(rootDirectory, $"Ocena-{packages_couter}" );
                LogRatingToFile(rating_item, Helper.FileTypeEnum.TXT, package_directory);
                LogRatingToFile(rating_item, Helper.FileTypeEnum.CSV, package_directory);
                LogRatingToFile(rating_item, Helper.FileTypeEnum.HTML, package_directory);

            }
            SerializeToFile(ratingList, Path.Combine(rootDirectory, "PlikSerializacji.xml"));
            Process.Start(rootDirectory);
            //Process fileProc = new Process(rootDirectory);
            //fileProc.EnableRaisingEvents = true;
            //fileProc.StartInfo.FileName = path;
            //fileProc.Start();
        }
    }
}
