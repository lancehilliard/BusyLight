using System;
using System.ServiceProcess;

namespace BusyLight.LightService {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            if (Environment.UserInteractive)  
            {  
                var service = new Service1();
                service.TestStartupAndStop();
            }  
            else {
                var servicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
