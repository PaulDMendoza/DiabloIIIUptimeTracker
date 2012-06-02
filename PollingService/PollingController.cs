using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PollingService
{
    public class PollingController
    {
        public List<IPoller> Pollers { get; set; }

        public PollingController()
        {
            var db = new UptimeData.UptimeDB(ConfigurationManager.AppSettings["UptimeDBFolder"]);

            Pollers = new List<IPoller>() { new BlizzardServerStatusPoller(db) };
        }
        
        public void PollAllPollersOnce()
        {
            foreach (var p in Pollers)
            {
                try
                {
                    p.Poll();
                }
                catch (Exception exFailedPolling)
                {
                    EventLog.WriteEntry("Uptime Polling Service", "Failed while polling with the poller " + p.GetType().FullName + " with the error " + exFailedPolling.ToString(), EventLogEntryType.Warning);
                }
            }
        }
    }
}
