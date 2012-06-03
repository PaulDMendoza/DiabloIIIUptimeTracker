using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Site.Models.Home
{
    public class RecentPollsViewModel
    {
        public List<UptimeData.PollCategoryValue> PollValues { get; set; }

        public RecentPollsViewModel(UptimeData.UptimeDB db)
        {
            var recentPolls = db.GetMostRecentValues(200);
            PollValues = recentPolls.ToList();
        }

    }
}