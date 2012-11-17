using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Site.Models.Home;

namespace Site.Tests
{
    [TestClass]
    public class UptimeDataJsonModelTests
    {
        /// <summary>
        /// Just ensures the model fills without crashing.
        /// </summary>
        [TestMethod]        
        public void FullFillTest()
        {
            
            var uptimeDB = new UptimeData.UptimeDB();
            var uptimeDataJson = new UptimeDataJsonModel(uptimeDB);
            uptimeDataJson.Fill();
        }
    }
}
