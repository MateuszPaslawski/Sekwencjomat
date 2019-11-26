using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Sekwencjomat.Models
{
    public static class Logger
    {
        private static string LoggingDir = Path.Combine(Helper.ExecutionPath, "Sekwencjomat-Wyniki");

        public static void LogRatingToCSV(Rating rating)
        {
            Directory.CreateDirectory(LoggingDir);

            int i = 1;

            while(File.Exists(Path.Combine(LoggingDir, $"{rating.PlaybackTechnique.ToString()}{DateTime.Now.ToString(@"dd-MM-yyyy")}_{i}.csv")))
            {
                i++;
            }


            int lp_counter = 1;

            using (StreamWriter file =
            new StreamWriter(Path.Combine(LoggingDir, $"{rating.PlaybackTechnique.ToString()}_{i}.csv"), false, Encoding.UTF8))
            {
                file.WriteLine($"Data;{rating.DateTimeString}");
                file.WriteLine($"Technika;{rating.PlaybackTechnique}");
                file.WriteLine($"Kolejność;{rating.PlaybackMode}");
                file.WriteLine($"Czas na ocenę [s];{rating.RatingSeconds}");
                file.WriteLine($"Czas trwania badania;{rating.RatingTimeSpanToString}");

                if(rating.PlaybackTechnique == Helper.PlaybackTechnique.DCR)
                    file.WriteLine($"Plik referencyjny;;{rating.ReferenceVideoPath}");

                file.WriteLine($"lp;Ocena;Plik;Bitrate [kB/s];Rozdzielczosc;FPS;");

                foreach (MediaFile mf in rating.FilesListWithGrades)
                {
                    file.WriteLine($"{lp_counter};{mf.UserGrade};{mf.Name};{mf.Bitrate};{mf.Resolution};{mf.FPS}");
                    lp_counter++;
                }
            }

        }
    }
}
