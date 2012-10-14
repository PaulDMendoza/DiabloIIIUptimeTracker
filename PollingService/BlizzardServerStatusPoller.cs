using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using UptimeData;

namespace PollingService
{
    public class BlizzardServerStatusPoller : IPoller
    {
        private UptimeData.UptimeDB DB { get; set; }
        private List<PollCategory> Categories { get; set; }

        public BlizzardServerStatusPoller(UptimeData.UptimeDB db)
        {
            DB = db;
            Categories = DB.GetPollCategories().ToList();
        }

        public void Poll()
        {
            var htmlText = GetBlizzardStatusPageHTML();

            var html = CreateHTMLParser(htmlText);
            var regionNodes = GetRegionElements(html).ToList();
            foreach (var region in regionNodes)
            {
                ParseRegionElement(region);
            }
            DB.SaveChanges();
        }

        private HtmlNodeCollection GetRegionElements(HtmlDocument html)
        {
            return html.DocumentNode.SelectNodes("//div[@class=\"server-status\"]//div[@class=\"box\"]");
        }

        private HtmlDocument CreateHTMLParser(string htmlText)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlText);
            return html;
        }

        private void ParseRegionElement(HtmlNode region)
        {
            var regionTitle = region.SelectSingleNode("h3").InnerText;

            foreach (var server in region.SelectNodes(".//div[@class=\"server\" or @class=\"server alt\"]"))
            {
                var serverName = server.SelectSingleNode(".//div[@class=\"server-name\"]").InnerText.Trim();

                var pollCategoryValue = new PollCategoryValue();
                var possibleCategoryMatch = Categories.FirstOrDefault(p => string.Compare(p.Region, regionTitle, true) == 0 && string.Compare(p.ServerCategory, serverName) == 0);
                if (possibleCategoryMatch == null)
                    continue;

                pollCategoryValue.CategoryID = possibleCategoryMatch.PollCategoryID;
                pollCategoryValue.Status = PollStatusType.Unknown;
                pollCategoryValue.CreatedTime = DateTime.Now;
                foreach (var div in server.SelectNodes("div"))
                {
                    if (div.OuterHtml.Contains("status-icon"))
                    {
                        pollCategoryValue.Status = div.OuterHtml.Contains("status-icon up") ? PollStatusType.Up : PollStatusType.Down;
                    }
                }
                DB.InsertPollCategoryValue(pollCategoryValue);
            }
        }

        private string GetBlizzardStatusPageHTML()
        {
            var web = (HttpWebRequest) HttpWebRequest.Create("http://us.battle.net/d3/en/status");
            var resp = web.GetResponse();
            Stream receiveStream = resp.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string htmlText = readStream.ReadToEnd();
            readStream.Close();
            return htmlText;
        }
    }
}
