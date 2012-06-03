using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UptimeData
{
    public class PollCategoryValue
    {
        public PollStatusType Status { get; set; }
        public PollCategory Category { get; set; }

        public int PollCategoryValueID { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
    }
}
