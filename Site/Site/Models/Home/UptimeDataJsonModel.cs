using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UptimeData;
using Site.Models.Home.UptimDataJsonModel;
using System.Data.Objects.SqlClient;


namespace Site.Models.Home
{
    public class UptimeDataJsonModel
    {
        public List<CategoryViewModel> CategoryViewModels { get; set; }
        private UptimeDB DB { get; set; }
        public UptimeDataJsonModel(UptimeDB db)
        {
            CategoryViewModels = db.GetPollCategories().Select(pc => new CategoryViewModel(pc)).ToList();

            DB = db;
        }

        public void Fill()
        {           
            foreach (var category in CategoryViewModels)
            {
                FillCategoryViewModel(category);
            }

            CategoryViewModels = CategoryViewModels.Where(p => p.HasReads).ToList();
        }

        private void FillCategoryViewModel(CategoryViewModel categoryViewModel)
        {            
            categoryViewModel.MostRecentRead = GetMostRecentRead(categoryViewModel);
            categoryViewModel.StatusMessage = GetStatusMessage(categoryViewModel);            
            categoryViewModel.UptimeLast24Hours = GetUptimeForLast24Hours(categoryViewModel);
            categoryViewModel.Uptime30Days = GetUptimeLast30Days(categoryViewModel);
            categoryViewModel.DowntimePercentage = GetDowntimePercentage(categoryViewModel);
            categoryViewModel.DayOfWeek = GetDayOfWeekDownMost(categoryViewModel);
            categoryViewModel.HourOfDay = GetHourOfDayDownTheMost(categoryViewModel);

            categoryViewModel.HasReads = true;
        }

        private PollCategoryValueJSON GetMostRecentRead(CategoryViewModel category)
        {
            var recentReadingValue = DB.PollCategoryValue
                .Where(v => v.CategoryID == category.PollCategoryID)
                .OrderByDescending(p => p.CreatedTime)
                .FirstOrDefault();
            if (recentReadingValue == null)
                return null;
            return new PollCategoryValueJSON(recentReadingValue);
        }

        private int? GetHourOfDayDownTheMost(CategoryViewModel category)
        {
            var afterDate = DateTime.Now.AddMonths(-1);
            var downtimeHour = DB.PollCategoryValue
                .Where(p => p.CategoryID == category.PollCategoryID && p.CreatedTime > afterDate && p.Status == PollStatusType.Down)
                .GroupBy(r => SqlFunctions.DatePart("hour", r.CreatedTime))
                .OrderByDescending(g => g.Count())
                .Select(r => r.Key)
                .FirstOrDefault();

            return downtimeHour ?? 4;
        }

        private string GetDayOfWeekDownMost(CategoryViewModel category)
        {
            
            var afterDate = DateTime.Now.AddMonths(-1);
            var downtimeDayOfWeekGroupOfReads = DB.PollCategoryValue
                .Where(p => p.CategoryID == category.PollCategoryID && p.CreatedTime > afterDate && p.Status == PollStatusType.Down)
                .GroupBy(r => SqlFunctions.DatePart("weekday", r.CreatedTime))
                .OrderByDescending(g => g.Count())
                .Select(r=>r.Key)
                .FirstOrDefault();
            var x = downtimeDayOfWeekGroupOfReads != null ? Enum.ToObject(typeof(DayOfWeek), downtimeDayOfWeekGroupOfReads).ToString() : "Tuesday";
            return x;
        }

        private decimal GetDowntimePercentage(CategoryViewModel category)
        {
            var afterDate = DateTime.Now.AddMonths(-1);
            var queryForReadsAfterDate = DB.PollCategoryValue.Where(c => c.CategoryID == category.PollCategoryID && c.CreatedTime > afterDate);               
            var totalReadings = queryForReadsAfterDate.Count();
            var downReadings = queryForReadsAfterDate.Count(r=>r.Status == PollStatusType.Down);
            return (decimal)downReadings / totalReadings;
        }

        private List<UptimeInTimespan> GetUptimeLast30Days(CategoryViewModel category)
        {
            DateTimeOffset date30DaysAgo = DateTimeOffset.Now.AddDays(-31);

            string sqlFormat = @"
                    select  CAST(CreatedTime as DATE) as T,	  
	                     (select COUNT(0) from PollCategoryValue upValues 
			                    where CategoryID = '{0}' 
				                    AND DAY( CAST(v.CreatedTime as DATE)) = DAY( CAST(upValues.CreatedTime as DATE) ) 
                                    AND MONTH( CAST(v.CreatedTime as DATE)) = MONTH( CAST(upValues.CreatedTime as DATE) ) 
                                    AND YEAR( CAST(v.CreatedTime as DATE)) = YEAR( CAST(upValues.CreatedTime as DATE) ) 
				                    AND Status = 1)
		                    / CAST( COUNT(0) as decimal(18, 5)) as P
                    from PollCategoryValue v
                    where CategoryID = '{0}' 
	                    AND CreatedTime >= DATEADD(month, -1, GETDATE())
                    group by CAST(CreatedTime as DATE)
                    order by T";
            string sql = string.Format(sqlFormat, category.PollCategoryID);
                        
            var uptimeStats = DB.Database.SqlQuery<UptimeInTimespan>(sql).ToList();               
            return uptimeStats;
        }

        private List<UptimeInTimespan> GetUptimeForLast24Hours(CategoryViewModel category)
        {
            var oneDayAgo = DateTimeOffset.Now.AddDays(-1);
            return DB.PollCategoryValue
                        .Where(v=>v.CategoryID == category.PollCategoryID)
                        .Where(r => r.CreatedTime >= oneDayAgo)
                        .ToList()
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

        private string GetStatusMessage(CategoryViewModel matchingCategoryViewModel)
        {
            var statusMessage = "";
            var lastChange = DB.PollCategoryValue.Where(v=>v.CategoryID == matchingCategoryViewModel.PollCategoryID)
                .Where(p => (p.Status == PollStatusType.Down && matchingCategoryViewModel.MostRecentRead.Status != PollStatusType.Down) ||
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
    }
}