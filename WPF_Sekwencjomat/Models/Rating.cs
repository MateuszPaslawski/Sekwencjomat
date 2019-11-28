using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sekwencjomat.Models
{
    public class Rating
    {
        public string DateTimeString
        {
            get
            {
                return DateTime.Now.ToString(@"yyyy-MM-dd HH:mm");
            }
        }

        public string RatingTimeSpanToString
        {
            get
            {
                return RatingTimeSpan.ToString(@"hh\h\ mm\m\ ss\s");
            }
        }

        public TimeSpan RatingTimeSpan { get; set; }
        public int RatingSeconds { get; set; }
        public string ReferenceVideoPath { get; set; }
        public Helper.PlaybackMode PlaybackMode { get; set; }
        public Helper.PlaybackScale PlaybackScale { get; set; }
        public List<MediaFile> FilesListWithGrades { get; set; }
    }
}
