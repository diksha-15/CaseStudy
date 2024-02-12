using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Video_Streaming.DbEntity
{
    public class VideoInformation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string VideoUrl { get; set; }
        public int Version { get; set; }
        public double FileSize { get; set; }
        public int IsDeleted { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}