using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using UptimeData;

namespace Site.Models.Home
{
    public class RecentPollsViewModel
    {
        public class ValueItem
        {
            public DateTimeOffset Time { get; set; }
            public string Region { get; set; }
            public string Server { get; set; }
            public PollStatusType Status { get; set; }
        }

        public List<ValueItem> PollValues { get; set; }

        public RecentPollsViewModel(UptimeData.UptimeDB db)
        {
            var recentPolls = db.PollCategoryValue
                .OrderByDescending(p => p.CreatedTime)
                .Take(200)
                .Select(v => new ValueItem()
                {
                    Region = v.PollCategory.Region,
                    Server = v.PollCategory.ServerCategory,
                    Status = v.Status,
                    Time = v.CreatedTime
                });
            PollValues = recentPolls.ToList();
        }

    }
}