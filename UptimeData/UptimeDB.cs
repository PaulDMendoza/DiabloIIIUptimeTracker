using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace UptimeData
{
    public class UptimeDB
    {
        string dbConnectionString;
        public UptimeDB(string dbContainingFolder)
        {
            dbConnectionString = "Data Source=" + Path.Combine(dbContainingFolder, "UptimeDB.s3db");
        }

        public IEnumerable<PollCategory> GetPollCategories()
        {
            using (var conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();
                var comm = conn.CreateCommand();
                String query = "select *";
                query += "from PollCategory;";
                comm.CommandText = query;
                using (var reader = comm.ExecuteReader())
                {
                    var categories = new List<PollCategory>();
                    while (reader.Read())
                    {
                        var c = new PollCategory();
                        c.Region = reader["Region"].ToString();
                        c.ServerCategory = reader["ServerCategory"].ToString();
                        c.PollCategoryID = reader.GetInt32(reader.GetOrdinal("PollCategoryID"));
                        categories.Add(c);
                    }
                    return categories;
                }
            }
        }


        public void InsertPollCategoryValue(PollCategoryValue pollCategoryValue)
        {
            using (var conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "insert into PollCategoryValue (StatusCode, PollCategoryID) VALUES (@StatusCode, @PollCategoryID);";
                comm.Parameters.AddWithValue("@StatusCode", (int)pollCategoryValue.Status);
                comm.Parameters.AddWithValue("@PollCategoryID", pollCategoryValue.Category.PollCategoryID);
                comm.ExecuteNonQuery();
            }
        }

        public List<PollCategoryValue> GetValuesSince(DateTime startTime)
        {
            String query = "select * from PollCategoryValue;";
            return ReadPollCategoryValues(query);
        }

        public List<PollCategoryValue> GetMostRecentValues(int count)
        {
            String query = "select * from PollCategoryValue order by CreatedTime desc, PollCategoryID asc limit " + count + ";";
            return ReadPollCategoryValues(query);
        }

        private List<PollCategoryValue> ReadPollCategoryValues(string query)
        {
            var categories = GetPollCategories().ToDictionary(p => p.PollCategoryID);
            using (var conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();
                var comm = conn.CreateCommand();
                
                comm.CommandText = query;
                using (var reader = comm.ExecuteReader())
                {
                    int idOrdinal = reader.GetOrdinal("PollCategoryValueID");
                    int statusCodeOridinal = reader.GetOrdinal("StatusCode");
                    int categoryIDOridinal = reader.GetOrdinal("PollCategoryID");
                    int createdTimeOrdinal = reader.GetOrdinal("CreatedTime");

                    var values = new List<PollCategoryValue>();
                    while (reader.Read())
                    {
                        var value = new PollCategoryValue();
                        value.PollCategoryValueID = reader.GetInt32(idOrdinal);
                        value.Status = (PollStatusType) Enum.ToObject(typeof (PollStatusType), reader.GetInt32(statusCodeOridinal));
                        value.Category = categories[reader.GetInt32(categoryIDOridinal)];
                        value.CreatedTime = reader.GetDateTime(createdTimeOrdinal);
                        values.Add(value);
                    }
                    return values;
                }
            }
        }
    }
}