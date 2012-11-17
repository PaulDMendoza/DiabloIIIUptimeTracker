using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UptimeData;
using System.Linq;
using PollingService;
using System.Data.Entity;


namespace Site.Tests
{
    [TestClass]
    [Ignore]
    public class UptimeDBTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var db = new UptimeDB();
            //db.Database.ExecuteSqlCommand("delete from PollCategoryValue");
            //db.Database.ExecuteSqlCommand("delete from PollCategory");
        }

        [TestMethod]        
        public void CreateDB()
        {
            var db = new UptimeDB();
            db.Recreate();
        }

        [TestMethod]
        public void InsertResult()
        {
            var category = new PollCategory()
            {
                PollCategoryID = Guid.NewGuid(),
                Region = "Zimbabwae",
                ServerCategory = "Something awesome"
            };

            var db = new UptimeDB();
            db.PollCategories.Add(category);

            var value = new PollCategoryValue()
            {
                CategoryID = category.PollCategoryID,
                CreatedTime = DateTime.Now,
                Status = PollStatusType.Up
            };
            db.PollCategoryValue.Add(value);

            var totalInserted = db.SaveChanges();
            Assert.AreEqual(2, totalInserted);

            db.PollCategoryValue.Remove(value);
            db.PollCategories.Remove(category);
            db.SaveChanges();
        }
        
        [TestMethod]
        public void TestPoll()
        {
            var db = new UptimeDB();

            var originalValueCount = db.PollCategoryValue.Count();

            var poller = new PollingController(db, (ex) =>
            {
                Assert.Fail(ex.ToString());
            });
            poller.PollAllPollersOnce();

            var db2 = new UptimeDB();
            var resultCount = db2.PollCategoryValue.Count();
            if (resultCount <= originalValueCount)
                Assert.Fail("Only found {0} values in the database and there were originally {1} so there wasn't enough of an increase after polling.", resultCount, originalValueCount);
        }        
    }
}
