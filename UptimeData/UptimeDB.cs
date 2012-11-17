using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Configuration;


namespace UptimeData
{   


    public class UptimeDB : DbContext
    {
        public UptimeDB()
            : base("UptimeDB")
        {
        }


        public void Recreate()
        {
            if (base.Database.Exists())
            {
                base.Database.Delete();
            }

            base.Database.Create();

            AddCategory("Americas", "Game Server");
            AddCategory("Americas", "Gold");
            AddCategory("Americas", "Hardcore");
            AddCategory("Americas", "USD");  

            AddCategory("Europe", "Game Server");
            AddCategory("Europe", "Gold");
            AddCategory("Europe", "Hardcore");
            AddCategory("Europe", "EUR");
            
            AddCategory("Asia", "Game Server");
            AddCategory("Asia", "Gold");
            AddCategory("Asia", "Hardcore");
            this.SaveChanges();
        }

        private void AddCategory(string region, string serverType) {
            this.PollCategories.Add(new PollCategory()
            {
                PollCategoryID = Guid.NewGuid(),
                Region = region,
                ServerCategory = serverType
            });
        }
                

        public IDbSet<PollCategory> PollCategories { get; set; }
        public IDbSet<PollCategoryValue> PollCategoryValue { get; set; }
        
        public IEnumerable<PollCategory> GetPollCategories()
        {
            return this.PollCategories.ToList();
        }

        public void InsertPollCategoryValue(PollCategoryValue pollCategoryValue)
        {
            this.PollCategoryValue.Add(pollCategoryValue);
        }

        public IEnumerable<PollCategoryValue> GetValuesSince(DateTime startTime)
        {
            return this.PollCategoryValue.Where(v => v.CreatedTime >= startTime);
        }

        public List<PollCategoryValue> GetMostRecentValues(int count)
        {
            return this.PollCategoryValue.OrderByDescending(p => p.CreatedTime).Take(count).ToList();
        }        
    }
}