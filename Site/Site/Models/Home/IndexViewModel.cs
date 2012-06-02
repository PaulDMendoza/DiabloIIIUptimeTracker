using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UptimeData;

namespace Site.Models.Home
{
    public class IndexViewModel
    {
        public UptimeDataJsonModel JsonModel { get; set; }

        public IndexViewModel(UptimeDB db)
        {
            JsonModel = new UptimeDataJsonModel(db);
            
        }
    }
}