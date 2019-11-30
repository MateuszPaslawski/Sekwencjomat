using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sekwencjomat.Models
{
    public class Rating
    {
        public string DateTimeRatingString
        {
            get
            {
                return DateTimeRating.ToString(@"yyyy-MM-dd HH:mm");
            }
        }

        public string RatingTimeSpanString
        {
            get
            {
                return RatingTimeSpan.ToString(@"hh\h\ mm\m\ ss\s");
            }
        }

        public int GradesCount
        {
            get
            {
                Console.WriteLine($"GETTING: {FilesListWithGrades.Count}");
                return FilesListWithGrades.Count;
            }
        }

        public string PlaybackModeString
        {
            get
            {
                return Helper.PlaybackModeToString(PlaybackMode);
            }
        }

        public Rating()
        {
            DateTimeRating = DateTime.Now;
        }



        public DateTime DateTimeRating { get; set; }
        public TimeSpan RatingTimeSpan { get; set; }
        public int RatingSeconds { get; set; }
        public string ReferenceVideoPath { get; set; }
        public Helper.PlaybackModeEnum PlaybackMode { get; set; }
        public Helper.PlaybackScaleEnum PlaybackScale { get; set; }
        public List<MediaFile> FilesListWithGrades { get; set; }
    }
}
