using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace PollingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Any(a=>a == "Console"))
            {
                var p = new PollingController();
                p.PollAllPollersOnce();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                                    {
                                        new Service1()
                                    };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
