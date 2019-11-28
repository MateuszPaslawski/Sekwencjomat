using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sekwencjomat.Models
{
    public static class Logger
    {
        private static string nowDate { get { return DateTime.Now.ToString(@"dd-MM-yyyy"); } }
        private static string LoggingDir = Path.Combine(Helper.ExecutionPath, "Sekwencjomat-Wyniki");
        private static string LoggingDirWithDate = Path.Combine(LoggingDir, nowDate);

        public async static void LogRatingToTXT(Rating rating)
        {
            int i = 1;
            int lp_counter = 1;
            string extension = ".txt";
            string scale = rating.PlaybackScale.ToString();

            Directory.CreateDirectory(LoggingDirWithDate);

            while (File.Exists(Path.Combine(LoggingDirWithDate, $"{scale}_{i}{extension}")))
            {
                i++;
            }

            using (StreamWriter file = new StreamWriter(Path.Combine(LoggingDirWithDate, $"{rating.PlaybackScale.ToString()}_{i}{extension}"), false, Encoding.UTF8))
            {
                file.WriteLine($"{"Data:", -20}{rating.DateTimeString}");
                file.WriteLine($"{"Metoda MOS:", -20}{rating.PlaybackScale}");
                file.WriteLine($"{"Kolejność:", -20}{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                file.WriteLine($"{"Czas trwania:", -20}{rating.RatingTimeSpanToString}");
                file.WriteLine($"{"Czas na ocenę [s]:", -20}{rating.RatingSeconds}");
                file.WriteLine();

                if (rating.PlaybackScale == Helper.PlaybackScale.DCR)
                    file.WriteLine($"{"Plik referencyjny:", -20}{rating.ReferenceVideoPath}");

                file.WriteLine($"{"lp",5} | {"Ocena",5} | {"Bitrate [kB/s]",15} | {"Rozdzielczość",15} | {"FPS",5} | {"Rozmiar",10} | {"Ścieżka pliku",-10}");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine($"{lp_counter,5} | {mf.UserGrade,5} | {mf.Bitrate,15} | {mf.FrameSize,15} | {mf.FPS,5} | {mf.Size,10} | {mf.Path,-10}");
                    lp_counter++;
                }
            }

        }

        public static void LogRatingToCSV(Rating rating)
        {
            Directory.CreateDirectory(LoggingDirWithDate);

            int i = 1;
            int lp_counter = 1;
            string extension = ".csv";
            string scale = rating.PlaybackScale.ToString();

            while (File.Exists(Path.Combine(LoggingDirWithDate, $"{scale}_{i}{extension}")))
            {
                i++;
            }


            using (StreamWriter file =
            new StreamWriter(Path.Combine(LoggingDirWithDate, $"{rating.PlaybackScale.ToString()}_{i}{extension}"), false, Encoding.UTF8))
            {
                file.WriteLine($"Data;{rating.DateTimeString}");
                file.WriteLine($"Metoda MOS;{rating.PlaybackScale}");
                file.WriteLine($"Kolejność;{Helper.PlaybackModeToString(rating.PlaybackMode)}");
                file.WriteLine($"Czas trwania badania;{rating.RatingTimeSpanToString}");
                file.WriteLine($"Czas na ocenę [s];{rating.RatingSeconds}");
                file.WriteLine();
                if (rating.PlaybackScale == Helper.PlaybackScale.DCR)
                    file.WriteLine($"Plik referencyjny;;{rating.ReferenceVideoPath}");

                file.WriteLine($"lp;Ocena;Bitrate [kB/s];Rozdzielczosc;FPS;Rozmiar;Ścieżka Pliku");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine($"{lp_counter};{mf.UserGrade};{mf.Bitrate};{mf.FrameSize};{mf.FPS};{mf.Size};{mf.Path}");
                    lp_counter++;
                }
            }

        }

        public static void LogRatingToHTML(Rating rating)
        {
            Directory.CreateDirectory(LoggingDirWithDate);

            int i = 1;
            int lp_counter = 1;
            string extension = ".html";
            string scale = rating.PlaybackScale.ToString();

            while (File.Exists(Path.Combine(LoggingDirWithDate, $"{scale}_{i}{extension}")))
            {
                i++;
            }


            using (StreamWriter file =
            new StreamWriter(Path.Combine(LoggingDirWithDate, $"{rating.PlaybackScale.ToString()}_{i}{extension}"), false, Encoding.UTF8))
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

                file.WriteLine($"<p>Data: <b>{rating.DateTimeString}</b></p>");
                file.WriteLine($"<p>Metoda MOS: <b>{rating.PlaybackScale}</b></p>");
                file.WriteLine($"<p>Kolejność: <b>{Helper.PlaybackModeToString(rating.PlaybackMode)}</b></p>");
                file.WriteLine($"<p>Czas trwania: <b>{rating.RatingTimeSpanToString}</b></p>");
                file.WriteLine($"<p>Czas na ocenę [s]: <b>{rating.RatingSeconds}</b></p>");

                if (rating.PlaybackScale == Helper.PlaybackScale.DCR)
                    file.WriteLine($"<p>Plik referencyjny: <b>{rating.ReferenceVideoPath}</b></p>");

                file.WriteLine("<table style=\"width: 100 %\">");
                file.WriteLine("<tr>");
                file.WriteLine("<th>lp</th>");
                file.WriteLine("<th>Ocena</th>");
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

        }
    }
}
