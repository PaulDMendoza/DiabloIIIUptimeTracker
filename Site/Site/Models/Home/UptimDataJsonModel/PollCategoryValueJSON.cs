using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UptimeData;

namespace Site.Models.Home.UptimDataJsonModel
{
    public class PollCategoryValueJSON
    {
        private PollCategoryValue _value;
        public PollCategoryValueJSON(PollCategoryValue value)
        {
            _value = value;
        }

        public DateTimeOffset CreatedTime { get { return _value.CreatedTime; } }
        public Guid CategoryID { get { return _value.CategoryID; } }
        public PollStatusType Status { get { return _value.Status; } }

    }
}