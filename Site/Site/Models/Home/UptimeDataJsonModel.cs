using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UptimeData;


namespace Site.Models.Home
{
    public class UptimeDataJsonModel
    {
        public List<CategoryViewModel> CategoryViewModels { get; set; }
        public UptimeDataJsonModel(UptimeDB db)
        {
            CategoryViewModels = db.GetPollCategories().Select(pc => new CategoryViewModel(pc)).ToList();

            var allReads = db.GetValuesSince(DateTime.Now.AddMonths(-1));
            var pollsByCategory = allReads.GroupBy(a => a.CategoryID);
            foreach (var polls in pollsByCategory)
            {
                FillCategoryViewModel(polls);
            }

            CategoryViewModels = CategoryViewModels.Where(p => p.HasReads).ToList();
        }

        private void FillCategoryViewModel(IGrouping<Guid, PollCategoryValue> readingGroup)
        {
            var categoryViewModel = CategoryViewModels.First(p => p.PollCategoryID == readingGroup.Key);
            categoryViewModel.MostRecentRead = GetMostRecentRead(readingGroup);
            categoryViewModel.StatusMessage = GetStatusMessage(readingGroup, categoryViewModel);            
            categoryViewModel.UptimeLast24Hours = GetUptimeForLast24Hours(readingGroup);
            categoryViewModel.Uptime30Days = GetUptimeLast30Days(readingGroup);
            categoryViewModel.DowntimePercentage = GetDowntimePercentage(readingGroup);
            categoryViewModel.DayOfWeek = GetDayOfWeekDownMost(readingGroup);
            categoryViewModel.HourOfDay = GetHourOfDayDownTheMost(readingGroup);

            categoryViewModel.HasReads = true;
        }

        private static PollCategoryValueJSON GetMostRecentRead(IGrouping<Guid, PollCategoryValue> readingGroup)
        {
            return readingGroup.OrderByDescending(p => p.CreatedTime).Select(p => new PollCategoryValueJSON(p)).First();
        }

        private static int? GetHourOfDayDownTheMost(IGrouping<Guid, PollCategoryValue> readingGroup)
        {
            var downtimeHour = readingGroup.Where(p => p.Status == PollStatusType.Down).GroupBy(r => r.CreatedTime.Hour).OrderByDescending(g => g.Count()).FirstOrDefault();
            var x = downtimeHour != null ? (int?)downtimeHour.Key : 4;
            return x;
        }

        private static string GetDayOfWeekDownMost(IGrouping<Guid, PollCategoryValue> readingGroup)
        {
            var downtimeDayOfWeekGroupOfReads = readingGroup.Where(p => p.Status == PollStatusType.Down).GroupBy(r => r.CreatedTime.DayOfWeek).OrderByDescending(g => g.Count()).FirstOrDefault();
            var x = downtimeDayOfWeekGroupOfReads != null ? downtimeDayOfWeekGroupOfReads.Key.ToString() : "Tuesday";
            return x;
        }

        private static decimal GetDowntimePercentage(IGrouping<Guid, PollCategoryValue> readingGroup)
        {
            return (decimal)readingGroup.Count(p => p.Status == PollStatusType.Down) / readingGroup.Count();
        }

        private static List<UptimeInTimespan> GetUptimeLast30Days(IGrouping<Guid, PollCategoryValue> reads)
        {
            DateTimeOffset date30DaysAgo = DateTimeOffset.Now.AddDays(-31);
            var uptimeStats = reads
                .Where(r => r.CreatedTime >= date30DaysAgo)
                .GroupBy(p => p.CreatedTime.ToString("MM/dd/yyyy"))
                .Select(pollValuesInDay =>
                {
                    var singleTime = pollValuesInDay.First().CreatedTime;
                    return new UptimeInTimespan()
                    {
                        P = (decimal)pollValuesInDay.Count(v => v.Status == PollStatusType.Up) / pollValuesInDay.Count(),
                        T = singleTime.Date
                    };
                })
                .ToList();
            return uptimeStats;
        }

        private static List<UptimeInTimespan> GetUptimeForLast24Hours(IGrouping<Guid, PollCategoryValue> reads)
        {
            return reads
                            .Where(r => r.CreatedTime >= DateTimeOffset.Now.AddDays(-1))
                            .GroupBy(p => p.CreatedTime.ToString("MM/dd/yyyy HH"))
                            .Select(pollValuesInHour =>
                            {
                                var singleTime = pollValuesInHour.First().CreatedTime;
                                return new UptimeInTimespan()
                                {
                                    P = (decimal)pollValuesInHour.Count(v => v.Status == PollStatusType.Up) / pollValuesInHour.Count(),
                                    T = singleTime.Date.AddHours(singleTime.Hour)
                                };
                            })
                            .ToList();
        }

        private string GetStatusMessage(IGrouping<Guid, PollCategoryValue> readingGroup, CategoryViewModel matchingCategoryViewModel)
        {

            var statusMessage = "";
            var lastChange = readingGroup.Where(p => (p.Status == PollStatusType.Down && matchingCategoryViewModel.MostRecentRead.Status != PollStatusType.Down) ||
                (p.Status == PollStatusType.Up && matchingCategoryViewModel.MostRecentRead.Status != PollStatusType.Up))
                .OrderByDescending(p => p.CreatedTime)
                .FirstOrDefault();
            if (lastChange != null)
            {
                TimeSpan timeSinceLastStatusChange = lastChange.CreatedTime.Subtract(DateTime.Now);
                statusMessage = ConvertTimeSpanToFriendlyText(timeSinceLastStatusChange);
            }
            return statusMessage;
        }

        public string ConvertTimeSpanToFriendlyText(TimeSpan t)
        {
            if (Math.Abs(t.TotalMinutes) < 60)
            {
                return Math.Abs(t.TotalMinutes).ToString("0") + " minutes";
            }
            else if (Math.Abs(t.TotalHours) < 24)
            {
                return Math.Abs(t.TotalHours).ToString("0") + " hours";
            }
            return Math.Abs(t.TotalDays).ToString("0") + " days";
        }

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

        public class UptimeInTimespan
        {
            public decimal P { get; set; }
            public DateTime T { get; set; }
        }

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
}