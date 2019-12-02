using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sekwencjomat.Models
{
    [Serializable]
    [XmlInclude(typeof(MediaFile))]
    public class Rating
    {
        public string DateTimeRatingString
        {
            get
            {
                return DateTimeRating.ToString(@"yyyy-MM-dd HH:mm");
            }
        }

        public int GradesCount
        {
            get
            {
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

        public string DurationSecondsString
        {
            get
            {
                TimeSpan ts = TimeSpan.FromSeconds(DurationSeconds);
                return ts.ToString(@"hh\h\ mm\m\ ss\s");
            }
        }

        public Rating()
        {

        }



        public DateTime DateTimeRating { get; set; }
        public int RatingSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public string ReferenceVideoPath { get; set; }
        public Helper.PlaybackModeEnum PlaybackMode { get; set; }
        public Helper.PlaybackScaleEnum PlaybackScale { get; set; }
        [XmlArray("ListOfMediaFiles")]
        [XmlArrayItem("MediaFileItem")]
        public List<MediaFile> FilesListWithGrades { get; set; }
    }
}
