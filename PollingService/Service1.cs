using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PollingService
{
    public partial class Service1 : ServiceBase
    {
        Task mainThread = null;
        bool isCancelled = false;
        public Service1()
        {
            this.ServiceName = "Uptime Polling Service";
            this.EventLog.Log = "Application";
            
            this.CanStop = true;

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            mainThread = new Task(() =>
            {
                try
                {
                    Thread.Sleep(20 * 1000);
                    PollingController pollingController = new PollingController();
                    DateTime nextPollTime = DateTime.Now;
                    while (!isCancelled)
                    {
                        if (nextPollTime <= DateTime.Now)
                        {
                            pollingController.PollAllPollersOnce();
                            nextPollTime = DateTime.Now.AddMinutes(5);
                        }

                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    this.EventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                }
            });

            mainThread.Start();
        }

        protected override void OnStop()
        {
            isCancelled = true;
            if (mainThread.Status == TaskStatus.Running)
            {
                mainThread.Wait(2000);
            }
            else
            {
                if (mainThread.IsFaulted)
                {
                    return;
                }
            }
        }
    }
}
