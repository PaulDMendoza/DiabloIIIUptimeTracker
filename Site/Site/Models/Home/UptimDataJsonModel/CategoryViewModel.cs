using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UptimeData;

namespace Site.Models.Home.UptimDataJsonModel
{
    public class CategoryViewModel
    {
        public CategoryViewModel(PollCategory pc)
        {
            ServerCategory = pc.ServerCategory;
            Region = pc.Region;
            PollCategoryID = pc.PollCategoryID;
        }

        public string ServerCategory { get; set; }
        public string Region { get; set; }
        public Guid PollCategoryID { get; set; }
        public string StatusMessage { get; set; }
        public PollCategoryValueJSON MostRecentRead { get; set; }
        public bool HasReads { get; set; }
        public string State
        {
            get
            {
                return MostRecentRead.Status.ToString();
            }
        }

        public List<UptimeInTimespan> UptimeLast24Hours { get; set; }

        public List<UptimeInTimespan> Uptime30Days { get; set; }

        public decimal DowntimePercentage { get; set; }

        public string DayOfWeek { get; set; }

        public int? HourOfDay { get; set; }
    }
}