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

            var allReads = db.GetValuesSince(DateTime.Now.AddMonths(-6));
            var readingsByCategory = allReads.GroupBy(a => a.Category);
            foreach (var readingGroup in readingsByCategory)
            {
                var categoryViewModel = CategoryViewModels.First(p => p.PollCategoryID == readingGroup.Key.PollCategoryID);

                categoryViewModel.MostRecentRead = readingGroup.OrderByDescending(p => p.CreatedTime).First();

                SetStatusMessage(readingGroup, categoryViewModel);

                categoryViewModel.UptimeLast24Hours = readingGroup
                    .Where(r => r.CreatedTime >= DateTime.Now.AddDays(-1))
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

                categoryViewModel.Uptime30Days = readingGroup
                    .Where(r => r.CreatedTime >= DateTime.Now.AddDays(-31))
                    .GroupBy(p => p.CreatedTime.ToString("MM/dd/yyyy"))
                    .Select(pollValuesInDay =>
                    {
                        var singleTime = pollValuesInDay.First().CreatedTime;
                        return new UptimeInTimespan()
                        {
                            P = (decimal)pollValuesInDay.Count(v => v.Status == PollStatusType.Up) / pollValuesInDay.Count(),
                            T = singleTime.Date.AddHours(singleTime.Hour)
                        };
                    })
                    .ToList();


                categoryViewModel.HasReads = true;
            }

            CategoryViewModels = CategoryViewModels.Where(p => p.HasReads).ToList();
        }

        private void SetStatusMessage(IGrouping<PollCategory, PollCategoryValue> readingGroup, CategoryViewModel matchingCategoryViewModel)
        {

            matchingCategoryViewModel.StatusMessage = "";
            var lastChange = readingGroup.Where(p => (p.Status == PollStatusType.Down && matchingCategoryViewModel.MostRecentRead.Status != PollStatusType.Down) ||
                (p.Status == PollStatusType.Up && matchingCategoryViewModel.MostRecentRead.Status != PollStatusType.Up))
                .OrderByDescending(p => p.CreatedTime)
                .FirstOrDefault();
            if (lastChange != null)
            {
                TimeSpan timeSinceLastStatusChange = lastChange.CreatedTime.Subtract(DateTime.Now);
                matchingCategoryViewModel.StatusMessage = ConvertTimeSpanToFriendlyText(timeSinceLastStatusChange);
            }
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
            public int PollCategoryID { get; set; }
            public string StatusMessage { get; set; }
            public PollCategoryValue MostRecentRead { get; set; }
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
        }

        public class UptimeInTimespan
        {
            public decimal P { get; set; }
            public DateTime T { get; set; }
        }
    }
}