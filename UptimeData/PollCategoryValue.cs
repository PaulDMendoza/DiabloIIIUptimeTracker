using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace UptimeData
{
    [Table("PollCategoryValue", Schema="dbo")]
    public class PollCategoryValue
    {
        [Key]
        [Column(Order = 0)]
        public DateTimeOffset CreatedTime { get; set; }
                
        [Key]
        [Column(Order=1)]
        public Guid CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public PollCategory PollCategory { get; set; }
                
        public PollStatusType Status
        {
            get;
            set;
        }

                
    }
}
