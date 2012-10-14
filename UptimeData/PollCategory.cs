using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;


namespace UptimeData
{
    [Table("PollCategory", Schema="dbo")]
    public class PollCategory
    {
        [Key]
        public Guid PollCategoryID { get; set; }
        public string ServerCategory { get; set; }
        public string Region { get; set; }

        public ICollection<PollCategoryValue> Values { get; set; }
    }
}