using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace PollingService
{
    public class PollingController
    {
        public List<IPoller> Pollers { get; set; }
        
        Action<Exception> _errorHandler = (ex)=> {
            try
            {
                EventLog.WriteEntry("Uptime Polling Service", "Failed while polling with the error " + ex.ToString(), EventLogEntryType.Warning);
            }
            catch (Exception exFailedWrite)
            {                
                
            }
            Console.WriteLine("Error: " + ex.ToString());
            
        };

        public PollingController()
        {
            var db = new UptimeData.UptimeDB();
            init(db);           
        }

        public PollingController(UptimeData.UptimeDB db, Action<Exception> errorHandler)
        {
            _errorHandler = errorHandler;
            init(db);
        }

        private void init(UptimeData.UptimeDB db)
        {
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
                    _errorHandler(exFailedPolling);
                }
            }
        }
    }
}
